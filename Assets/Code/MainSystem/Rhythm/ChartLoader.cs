using UnityEngine;
using System.Collections.Generic;
using Reflex.Attributes;

namespace Code.MainSystem.Rhythm
{
    [System.Serializable]
    public class ChartJsonData
    {
        public string title;
        public double bpm;
        public double offset;
        public List<NoteJsonData> notes;
    }

    [System.Serializable]
    public class NoteJsonData
    {
        public double time;
        public int lane;
        public int type;
    }

    public class ChartLoader : MonoBehaviour
    {
        [Inject] private Conductor _conductor;

        public List<NoteData> LoadChart(string jsonContent)
        {
            ChartJsonData chartData = JsonUtility.FromJson<ChartJsonData>(jsonContent);
            
            if (chartData == null)
            {
                Debug.LogError("ChartLoader: Failed to parse JSON.");
                return new List<NoteData>();
            }

            if (_conductor != null && chartData.bpm > 0)
            {
                _conductor.SetBpm(chartData.bpm);
            }

            List<NoteData> notes = new List<NoteData>();
            foreach (var noteJson in chartData.notes)
            {
                double finalTime = noteJson.time + chartData.offset; 
                notes.Add(new NoteData(finalTime, noteJson.lane, noteJson.type));
            }

            notes.Sort((a, b) => a.Time.CompareTo(b.Time));

            return notes;
        }

        public List<NoteData> LoadTestChart()
        {
            string json = @"
            {
              ""title"": ""Test Song"",
              ""bpm"": 120,
              ""offset"": 0.0,
              ""notes"": [
                { ""time"": 2.0, ""lane"": 0, ""type"": 0 },
                { ""time"": 2.5, ""lane"": 1, ""type"": 0 },
                { ""time"": 3.0, ""lane"": 2, ""type"": 0 },
                { ""time"": 3.5, ""lane"": 3, ""type"": 0 },
                { ""time"": 4.0, ""lane"": 3, ""type"": 0 },
                { ""time"": 4.5, ""lane"": 2, ""type"": 0 },
                { ""time"": 5.0, ""lane"": 1, ""type"": 0 },
                { ""time"": 5.5, ""lane"": 0, ""type"": 0 }
              ]
            }";
            return LoadChart(json);
        }

        public List<NoteData> LoadChartFromResources(string path)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(path);
            if (textAsset == null)
            {
                Debug.LogWarning($"ChartLoader: Could not find chart at path: {path}");
                return new List<NoteData>();
            }
            return LoadChart(textAsset.text);
        }

        public List<NoteData> CombineCharts(List<List<NoteData>> allCharts)
        {
            List<NoteData> combined = new List<NoteData>();
            foreach (var chart in allCharts)
            {
                combined.AddRange(chart);
            }
            
            // Sort by time to ensure correct playback order
            combined.Sort((a, b) => a.Time.CompareTo(b.Time));
            
            return combined;
        }
    }
}