using System;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Rhythm
{
    [Serializable]
    public struct RhythmEventDuration
    {
        public bool HasDuration;
        public float Seconds;

        public RhythmEventDuration(bool has, float secs = 0)
        {
            HasDuration = has;
            Seconds = secs;
        }
    }
    
    [Serializable]
    public class RhythmEvent
    {
        public float TimeSeconds => _timeSeconds;
        public List<RhythmData> RhythmData => _rhythmData;
        public float DurationSeconds => _duration.Seconds;
        public bool HasDuration => _duration.HasDuration;
        
        private float _timeSeconds;
        private RhythmEventDuration _duration;
        private List<RhythmData> _rhythmData; //metadata for this event
        public RhythmData GetData<T>()
        {
            foreach (RhythmData r in _rhythmData)
            {
                if (r is T)
                {
                    return r;
                }
            }
            return null;
        }

        public RhythmEvent(float s, float dur = 0)
        {
            _timeSeconds = s;
            _rhythmData = new List<RhythmData>();
            _duration = new RhythmEventDuration(dur <= float.Epsilon, dur);
        }

        public void SetTime(float s)
        {
            _timeSeconds = s;
        }

        public void SetDuration(float dur)
        {
            _duration = new RhythmEventDuration(dur <= float.Epsilon, dur);
        }

        public void AddData(RhythmData d)
        {
            _rhythmData.Add(d);
        }
        
        public void RemoveData(RhythmData d)
        {
            _rhythmData.Remove(d);
        }

        public bool Overlaps(RhythmEvent other, float error)
        {
            return Contains(other, error) || other.Contains(this, error);
        }

        private bool Contains(RhythmEvent other, float error)
        {
            float leftBound = TimeSeconds - error;
            float rightBound = TimeSeconds + error;
            return other.TimeSeconds >= leftBound && other.TimeSeconds <= rightBound;
        }
    }
}