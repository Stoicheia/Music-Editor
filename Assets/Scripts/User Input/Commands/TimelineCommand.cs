using RhythmEngine;
using UI;
using Unity.VisualScripting;
using UnityEngine;

namespace DefaultNamespace.Input
{
    public abstract class TimelineCommand
    {
        public abstract void Apply(TimelineUI ui, EditorEngine engine);

        public virtual void Redo(TimelineUI ui, EditorEngine engine)
        {
            Apply(ui, engine);
        }
        public abstract void Undo(TimelineUI ui, EditorEngine engine);
    }
}