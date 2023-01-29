using System;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;

namespace Rhythm
{
    [Serializable]
    public class RhythmEvent
    {
        public float TimeSeconds => _timeSeconds;
        public int TypeID => _typeID;
        public List<RhythmData> RhythmData => _rhythmData;
        
        private float _timeSeconds;
        private List<RhythmData> _rhythmData;
        private int _typeID;

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
    }
}