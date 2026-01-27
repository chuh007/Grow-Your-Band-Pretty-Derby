using UnityEngine;
using System.Collections.Generic;
using Reflex.Attributes;
using Cysharp.Threading.Tasks;
using Code.Core.Addressable;

using Code.MainSystem.Rhythm.Data;
using Code.MainSystem.Rhythm.Audio;

namespace Code.MainSystem.Rhythm.Notes
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

        public async UniTask<List<NoteData>> LoadChartAsync(string key)
        {
            if (GameResourceManager.Instance == null)
            {
                Debug.LogError("ChartLoader: GameResourceManager Instance is null!");
                return new List<NoteData>();
            }

            try
            {
                TextAsset textAsset = await GameResourceManager.Instance.LoadAsync<TextAsset>(key);
                if (textAsset == null)
                {
                    Debug.LogWarning($"ChartLoader: Failed to load chart asset at key: {key}");
                    return new List<NoteData>();
                }
                return LoadChart(textAsset.text);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"ChartLoader: Exception loading chart {key}: {ex.Message}");
                return new List<NoteData>();
            }
        }
    }
}