using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Code.MainSystem.StatSystem.Manager;

namespace Code.MainSystem.Rhythm
{
    public static class ConcertChartBuilder
    {
        // 중복으로 간주할 시간 오차 (초 단위, 2ms)
        private const double MERGE_EPSILON = 0.002d;
        
        // 채보 파일이 위치한 Resources 하위 폴더 경로
        private const string CHART_PATH_PREFIX = "Charts/";

        public static List<NoteData> Build(ChartLoader chartLoader, string songId, List<MemberType> memberRoles)
        {
            List<NoteData> rawNotes = new List<NoteData>();

            if (chartLoader == null)
            {
                Debug.LogError("[ConcertChartBuilder] ChartLoader is null!");
                return rawNotes;
            }

            // 1. 플레이어(Main) 차트 로드 (MemberId = 0)
            // 파일명 예시: Resources/Charts/Song001_Main
            string mainChartPath = $"{CHART_PATH_PREFIX}{songId}_Main";
            var playerNotes = chartLoader.LoadChartFromResources(mainChartPath);
            
            if (playerNotes != null && playerNotes.Count > 0)
            {
                foreach (var note in playerNotes)
                {
                    note.MemberId = 0; // 0번은 항상 플레이어
                    rawNotes.Add(note);
                }
                Debug.Log($"[ConcertChartBuilder] Loaded Main Chart: {mainChartPath} ({playerNotes.Count} notes)");
            }
            else
            {
                Debug.LogWarning($"[ConcertChartBuilder] Main chart not found or empty at: {mainChartPath}");
            }

            // 2. 멤버별 차트 로드 (MemberId = 1 ~ N)
            for (int i = 0; i < memberRoles.Count; i++)
            {
                MemberType role = memberRoles[i];
                int assignedMemberId = i + 1; // 1번부터 순차 할당

                // 파일명 예시: Resources/Charts/Song001_Bass
                string roleChartPath = $"{CHART_PATH_PREFIX}{songId}_{role}";
                var memberNotes = chartLoader.LoadChartFromResources(roleChartPath);
                
                if (memberNotes != null && memberNotes.Count > 0)
                {
                    foreach (var note in memberNotes)
                    {
                        note.MemberId = assignedMemberId;
                        rawNotes.Add(note);
                    }
                    Debug.Log($"[ConcertChartBuilder] Loaded Member {assignedMemberId} ({role}) Chart: {roleChartPath} ({memberNotes.Count} notes)");
                }
                else
                {
                    // 해당 악기 파트가 없는 경우도 있을 수 있으므로 LogWarning 대신 Log 정보로 남김
                    Debug.Log($"[ConcertChartBuilder] No chart for {role} at: {roleChartPath}");
                }
            }

            // 3. 중복 제거 및 정렬 로직 호출
            return ResolveOverlaps(rawNotes);
        }

        private static List<NoteData> ResolveOverlaps(List<NoteData> notes)
        {
            // [정렬 중요] 
            // 1순위: 시간(Time) 오름차순
            // 2순위: 레인(Lane) 오름차순
            // 3순위: MemberId 오름차순 (0번인 플레이어가 가장 앞에 오도록!)
            notes.Sort((a, b) =>
            {
                int t = a.Time.CompareTo(b.Time);
                if (t != 0) return t;

                int l = a.LaneIndex.CompareTo(b.LaneIndex);
                if (l != 0) return l;

                return a.MemberId.CompareTo(b.MemberId);
            });

            List<NoteData> cleanList = new List<NoteData>();

            if (notes.Count == 0) return cleanList;

            // 첫 번째 노트 추가
            cleanList.Add(notes[0]);

            for (int i = 1; i < notes.Count; i++)
            {
                NoteData current = notes[i];
                NoteData prev = cleanList[cleanList.Count - 1];

                bool isSameLane = current.LaneIndex == prev.LaneIndex;
                bool isSameTime = System.Math.Abs(current.Time - prev.Time) <= MERGE_EPSILON;

                if (isSameLane && isSameTime)
                {
                    // [중복 발생!]
                    // 정렬 로직 덕분에 'prev'에는 MemberId가 더 낮은(우선순위가 높은) 노트가 이미 들어가 있음.
                    // 따라서 지금 들어오려는 'current' 노트(MemberId가 더 높은 NPC 노트)를 무시.
                    continue; 
                }

                cleanList.Add(current);
            }

            Debug.Log($"[ConcertChartBuilder] Merge Complete: Raw {notes.Count} -> Final {cleanList.Count}");
            return cleanList;
        }
    }
}
