﻿using System;
using System.Collections.Generic;
using System.Linq;
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
    public class RhythmEvent : ISerializationCallbackReceiver
    {
        public float TimeSeconds => _timeSeconds;
        public List<RhythmData> RhythmData => _rhythmData;
        public float DurationSeconds => _duration.Seconds;
        public bool HasDuration => _duration.HasDuration;

        public float Vertical
        {
            get => _vertical;
            set => _vertical = value;
        }

        [SerializeField] private float _timeSeconds;
        [SerializeField] private RhythmEventDuration _duration;
        [SerializeField] private float _vertical;
        private List<RhythmData> _rhythmData; //metadata for this event
        [SerializeField] private List<IntData> _intData;
        [SerializeField] private List<StringData> _stringData;
        [SerializeField] private List<FloatData> _floatData;
        public RhythmData GetData(string propName)
        {
            foreach (var r in _rhythmData)
            {
                if (r.PropertyName == propName)
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
            _intData = new List<IntData>();
            _stringData = new List<StringData>();
            _floatData = new List<FloatData>();
            _duration = new RhythmEventDuration(dur > float.Epsilon, dur);
        }

        public void SetTime(float s)
        {
            _timeSeconds = s;
        }

        public void SetDuration(float dur)
        {
            _duration = new RhythmEventDuration(dur > float.Epsilon, dur);
        }

        public void SetEndTime(float s)
        {
            SetDuration(s - TimeSeconds);
        }
        
        public void AddData(RhythmData d)
        {
            _rhythmData.Add(d);
            if(d is IntData idata) _intData.Add(idata);
            if(d is StringData sdata) _stringData.Add(sdata);
            if(d is FloatData fdata) _floatData.Add(fdata);
        }
        
        public void RemoveData(RhythmData d)
        {
            _rhythmData.Remove(d);
            if(d is IntData idata) _intData.Remove(idata);
            if(d is StringData sdata) _stringData.Remove(sdata);
            if(d is FloatData fdata) _floatData.Remove(fdata);
        }

        public bool WithinRange(float left, float right)
        {
            float start = TimeSeconds;
            float end = TimeSeconds + DurationSeconds;

            return !((start < left && end < left) || (start > right && end > right));
        }

        public bool Overlaps(RhythmEvent other, float error)
        {
            return Contains(other, error) || other.Contains(this, error);
        }

        private bool Contains(RhythmEvent other, float error)
        {
            float leftBound = TimeSeconds - error;
            float rightBound = TimeSeconds + DurationSeconds + error;
            return other.TimeSeconds >= leftBound && other.TimeSeconds <= rightBound;
        }

        public bool Contains(float other, float error)
        {
            float leftBound = TimeSeconds - error;
            float rightBound = TimeSeconds + DurationSeconds + error;
            return other >= leftBound && other <= rightBound;
        }

        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            foreach(var i in _intData) _rhythmData.Add(i);
            foreach(var s in _stringData) _rhythmData.Add(s);
        }
    }
}