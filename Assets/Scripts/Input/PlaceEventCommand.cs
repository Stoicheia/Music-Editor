﻿using Rhythm;
using RhythmEngine;
using UI;
using UnityEngine;

namespace DefaultNamespace.Input
{
    public class PlaceEventCommand : TimelineCommand
    {
        public EventNodeUI EventNode => _eventNode;
        
        private float _time;
        private float _vertical;
        private EventNodeUI _eventNode;
        
        public PlaceEventCommand(float time, float vertical)
        {
            _time = time;
            _vertical = vertical;
        }
        public override void Apply(TimelineUI ui, EditorEngine engine)
        {
            RhythmEvent newEvent = new RhythmEvent(_time);
            engine.AddEvent(newEvent);
            var newNode = ui.EventNodePool.Dequeue();
            ui.EventToNode[newEvent] = newNode;
            ui.EventToNode[newEvent].Vertical = _vertical;
            ui.EventToNode[newEvent].Event = newEvent;
            _eventNode = newNode;
        }

        public override void Undo(TimelineUI ui, EditorEngine engine)
        {
            ui.EventNodePool.Enqueue(_eventNode);
            ui.EventToNode.Remove(_eventNode.Event);
            engine.RemoveEvent(_eventNode.Event);
        }
    }
}