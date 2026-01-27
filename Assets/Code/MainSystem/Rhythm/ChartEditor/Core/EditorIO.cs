#if UNITY_EDITOR
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Code.MainSystem.Rhythm.Data;

namespace Code.MainSystem.Rhythm.ChartEditor.Core
{
    public static class EditorIO
    {
        public static void SaveChart(string songId, int memberId, List<NoteData> notes, double bpm)
        {
            string memberName = GetMemberName(memberId);
            string folderPath = Path.Combine(Application.dataPath, "Resources/RhythmGame/Chart", songId);
            
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            
            string filePath = Path.Combine(folderPath, $"{memberName}.json");
            
            foreach(var note in notes)
            {
                note.SequenceLength = 7;
                note.BeatIndex = 7;
            }
            
            ChartContainer container = new ChartContainer { notes = notes };
            string json = JsonUtility.ToJson(container, true);
            
            File.WriteAllText(filePath, json);
            Debug.Log($"Saved chart to {filePath}");
        }

        public static List<NoteData> LoadChart(string songId, int memberId)
        {
            string memberName = GetMemberName(memberId);
            string filePath = Path.Combine(Application.dataPath, "Resources/RhythmGame/Chart", songId, $"{memberName}.json");
            
            if (!File.Exists(filePath)) return new List<NoteData>();
            
            string json = File.ReadAllText(filePath);
            ChartContainer container = JsonUtility.FromJson<ChartContainer>(json);
            return container != null ? container.notes : new List<NoteData>();
        }

        public static string GetMemberName(int id)
        {
            switch(id)
            {
                case 0: return "Vocal";
                case 1: return "Guitar";
                case 2: return "Bass";
                case 3: return "Drum";
                case 4: return "Piano";
                default: return "Unknown";
            }
        }

        [System.Serializable]
        private class ChartContainer
        {
            public List<NoteData> notes;
        }
    }
}
#endif