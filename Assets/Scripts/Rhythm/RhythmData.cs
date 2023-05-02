using System;

namespace Rhythm
{
    [Serializable]
    public abstract class RhythmData
    {
        public string PropertyName;

        public bool IsType<T>()
        {
            return this is T;
        }
    }

    public class IntData : RhythmData
    {
        public int IntValue;
    }

    public class StringData : RhythmData
    {
        public string StringValue;
    }
}