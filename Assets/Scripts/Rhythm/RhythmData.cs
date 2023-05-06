using System;
using UnityEngine;

namespace Rhythm
{
    [Serializable]
    public abstract class RhythmData
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
    }

    [Serializable]
    public class StringData : RhythmData
    {
        [SerializeField] public string StringValue;
    }
}