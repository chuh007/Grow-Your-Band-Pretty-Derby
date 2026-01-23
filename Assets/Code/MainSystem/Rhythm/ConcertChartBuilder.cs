using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Code.MainSystem.StatSystem.Manager;
using Cysharp.Threading.Tasks;

namespace Code.MainSystem.Rhythm
{
    public static class ConcertChartBuilder
    {
        private const double MERGE_EPSILON = 0.002d;
        
        private const string CHART_KEY_FORMAT = "RhythmGame/Chart/{0}/{1}";

        public static async UniTask<List<NoteData>> BuildAsync(ChartLoader chartLoader, string songId, List<MemberType> memberRoles)
        {
            List<NoteData> rawNotes = new List<NoteData>();

            if (chartLoader == null)
            {
                Debug.LogError("[ConcertChartBuilder] ChartLoader is null!");
                return rawNotes;
            }

            List<UniTask<List<NoteData>>> tasks = new List<UniTask<List<NoteData>>>();
            List<int> memberIds = new List<int>();

            string mainKey = string.Format(CHART_KEY_FORMAT, songId, "Main");
            tasks.Add(chartLoader.LoadChartAsync(mainKey));
            memberIds.Add(0);

            for (int i = 0; i < memberRoles.Count; i++)
            {
                MemberType role = memberRoles[i];
                string roleKey = string.Format(CHART_KEY_FORMAT, songId, role);
                tasks.Add(chartLoader.LoadChartAsync(roleKey));
                memberIds.Add(i + 1);
            }

            var results = await UniTask.WhenAll(tasks);

            for (int i = 0; i < results.Length; i++)
            {
                var loadedNotes = results[i];
                int currentMemberId = memberIds[i];

                if (loadedNotes != null && loadedNotes.Count > 0)
                {
                    foreach (var note in loadedNotes)
                    {
                        note.MemberId = currentMemberId;
                        
                        note.SequenceLength = 7;
                        note.BeatIndex = 6;
                        
                        rawNotes.Add(note);
                    }
                    Debug.Log($"[ConcertChartBuilder] Loaded Chart for Member {currentMemberId}: {loadedNotes.Count} notes");
                }
            }

            return ResolveOverlaps(rawNotes);
        }

        private static List<NoteData> ResolveOverlaps(List<NoteData> notes)
        {
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

            cleanList.Add(notes[0]);

            for (int i = 1; i < notes.Count; i++)
            {
                NoteData current = notes[i];
                NoteData prev = cleanList[cleanList.Count - 1];

                bool isSameLane = current.LaneIndex == prev.LaneIndex;
                bool isSameTime = System.Math.Abs(current.Time - prev.Time) <= MERGE_EPSILON;

                if (isSameLane && isSameTime)
                {
                    continue; 
                }

                cleanList.Add(current);
            }

            Debug.Log($"[ConcertChartBuilder] Merge Complete: Raw {notes.Count} -> Final {cleanList.Count}");
            return cleanList;
        }
    }
}