using System;
using RhythmEngine;
using UnityEngine;

namespace UI
{
    public abstract class TimelineDrawerHelperUI : MonoBehaviour
    {
        [field: NonSerialized] public TimelineUI Timeline { get; set; }
        protected EditorEngine _engine;

        public virtual void Init(TimelineUI timeline, EditorEngine data)
        {
            Timeline = timeline;
            _engine = data;
        }

        public abstract void Clear();
        public abstract void Draw(EditorEngine data, Rect panel, float leftTime, float rightTime, int fromIndex = 0);
    }
}