using System;
using UnityEngine;

namespace Rhythm
{
    [Serializable]
    public abstract class RhythmData // Not using generics here to make JSON easier
    {
        [SerializeField] public string PropertyName;

        public bool IsType<T>()
        {
            return this is T;
        }
    }

    [Serializable]
    public class IntData : RhythmData
    {
        [SerializeField] public int IntValue;
        public IntData(int i, string name = "")
        {
            IntValue = i;
            PropertyName = name;
        }
    }

    [Serializable]
    public class StringData : RhythmData
    {
        [SerializeField] public string StringValue;

        public StringData(string s, string name = "")
        {
            StringValue = s;
            PropertyName = name;
        }
    }
    
    [Serializable]
    public class FloatData : RhythmData
    {
        [SerializeField] public float FloatValue;

        public FloatData(float f, string name = "")
        {
            FloatValue = f;
            PropertyName = name;
        }
    }
}