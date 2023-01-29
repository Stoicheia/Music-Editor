using System;
using System.Collections.Generic;
using Rhythm;
using UnityEngine;

namespace RhythmEngine
{
    [Serializable]
    public class LevelData
    {
        public List<RhythmEvent> Events => _events;
        private List<RhythmEvent> _events;
    }
}