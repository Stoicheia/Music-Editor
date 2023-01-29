using System;

namespace UI
{
    public enum EventType
    {
        Tick, Match
    }
    
    public class EventMarkerInfo : SelectInfo
    {
        public EventType Type;

        public EventMarkerInfo(EventType t)
        {
            Type = t;
        }
    }
}