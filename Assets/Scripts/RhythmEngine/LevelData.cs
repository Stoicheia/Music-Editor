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
        public List<RhythmEvent> Events => _events;
        private List<RhythmEvent> _events;
        
        private List<BpmChange> _bpmChanges;
        private List<TimeSignatureChange> _timeSigChanges;

        private float _defaultBpm;
        private TimeSignature _defaultTimeSignature;

        public LevelData(float defaultBpm, TimeSignature defaultTimeSignature)
        {
            _events = new List<RhythmEvent>();
            _bpmChanges = new List<BpmChange>();
            _timeSigChanges = new List<TimeSignatureChange>();
        
            AddBpmChange(0, defaultBpm);
            AddTimeSignatureChange(0, defaultTimeSignature);

            _defaultBpm = defaultBpm;
            _defaultTimeSignature = defaultTimeSignature;
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