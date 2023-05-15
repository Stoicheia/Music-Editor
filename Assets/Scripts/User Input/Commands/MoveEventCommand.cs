using RhythmEngine;
using UI;
using UnityEngine;

namespace DefaultNamespace.Input
{
    public class MoveEventCommand : TimelineCommand
    {
        private float _oldTimeSeconds;
        private float _oldDuration;
        private float _oldVertical;
        
        private float _newTimeSeconds;
        private float _newDuration;
        private float _newVertical;
        
        private EventNodeUI _eventNode;

        public MoveEventCommand(EventNodeUI e, float s, float d, float v)
        {
            _oldTimeSeconds = s;
            _oldDuration = d;
            _oldVertical = v;
            _eventNode = e;
        }
        public override void Apply(TimelineUI ui, EditorEngine engine)
        {
            _newTimeSeconds = _eventNode.Time;
            _newDuration = _eventNode.Event.DurationSeconds;
            _newVertical = _eventNode.Vertical;
        }

        public override void Undo(TimelineUI ui, EditorEngine engine)
        {
            _eventNode.Event.SetTime(_oldTimeSeconds);
            _eventNode.Event.SetDuration(_oldDuration);
            _eventNode.Event.Vertical = _oldVertical;
        }

        public override void Redo(TimelineUI ui, EditorEngine engine)
        {
             _eventNode.Event.SetTime(_newTimeSeconds);
            _eventNode.Event.SetDuration(_newDuration);
            _eventNode.Vertical = _newVertical;
        }
    }
}