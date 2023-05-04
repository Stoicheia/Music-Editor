using System;
using System.Collections.Generic;
using Rhythm;
using UnityEngine;

namespace RhythmEngine
{
    public class BpmChange
    {
        public float Time;
        public float Bpm;

        public BpmChange(float t, float b)
        {
            Time = t;
            Bpm = b;
        }
    }

    public class TimeSignatureChange
    {
        public float Time;
        public TimeSignature TimeSignature;

        public TimeSignatureChange(float t, TimeSignature ts)
        {
            Time = t;
            TimeSignature = ts;
        }
    }
    
    [Serializable]
    public class LevelData
    {
        [field: SerializeField] public SongAssetData SongData { get; set; }
        public List<RhythmEvent> Events => _events;
        [SerializeField] private List<RhythmEvent> _events;
        
        [SerializeField] private List<BpmChange> _bpmChanges;
        [SerializeField] private List<TimeSignatureChange> _timeSigChanges;

        private float _defaultBpm => SongData.DefaultBpm;
        private TimeSignature _defaultTimeSignature => SongData.DefaultTimeSignature;

        public LevelData(float defaultBpm, TimeSignature defaultTimeSignature)
        {
            _events = new List<RhythmEvent>();
            _bpmChanges = new List<BpmChange>();
            _timeSigChanges = new List<TimeSignatureChange>();
        
            AddBpmChange(0, defaultBpm);
            AddTimeSignatureChange(0, defaultTimeSignature);
        }

        public BpmChange AddBpmChange(float time, float bpm)
        {
            bpm = Mathf.Max(1, bpm);
            BpmChange change = new BpmChange(time, bpm);
            _bpmChanges.Add(change);
            _bpmChanges.Sort((x, y) => x.Time.CompareTo(y.Time));
            return change;
        }

        public TimeSignatureChange AddTimeSignatureChange(float time, TimeSignature signature)
        {
            TimeSignatureChange change = new TimeSignatureChange(time, signature);
            _timeSigChanges.Add(change);
            _timeSigChanges.Sort((x,y) => x.Time.CompareTo(y.Time));
            return change;
        }

        public void RemoveBpmChange(BpmChange change)
        {
            _bpmChanges.Remove(change);
        }

        public void RemoveTimeSignatureChange(TimeSignatureChange change)
        {
            _timeSigChanges.Remove(change);
        }

        public List<BpmChange> GetBpmChanges() => _bpmChanges;
        public List<TimeSignatureChange> GetTimeSigChanges() => _timeSigChanges;

        public void Clear()
        {
            _bpmChanges.Clear();
            _events.Clear();
            _timeSigChanges.Clear();
            
            AddBpmChange(0, _defaultBpm);
            AddTimeSignatureChange(0, _defaultTimeSignature);
        }
    }
}