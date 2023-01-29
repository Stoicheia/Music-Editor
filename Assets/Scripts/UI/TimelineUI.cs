using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class TimelineUI : MonoBehaviour, ISelectorInteractor
    {
        public SelectorUI Selector { get; set; }
        public SongSeekerUI SongSeeker { get; set; }
        
        [SerializeField] private float _focusSeconds;
        [Space]
        [SerializeField] private GameObject _tickEventPrefab;
        [SerializeField] private GameObject _matchEventPrefab;
        [SerializeField] private TextMeshProUGUI _timelineTimePrefab;
        [SerializeField] private GameObject _thinGridlinePrefab;
        [SerializeField] private GameObject _thickGridlinePrefab;
        

        private float _leftTime;
        private float _rightTime;

        private void Update()
        {
            UpdateTimelinePosition();
        }
        
        public void Place(SelectInfo info)
        {
            EventMarkerInfo emi = info as EventMarkerInfo;
            if (emi == null) return;
        }

        public void SelectTickEvent()
        {
            EventMarkerInfo info = new EventMarkerInfo(EventType.Tick);
            Selector.Select(_tickEventPrefab, info);
        }

        public void SelectMatchEvent()
        {
            EventMarkerInfo info = new EventMarkerInfo(EventType.Match);
            Selector.Select(_matchEventPrefab, info);
        }
        
        private void UpdateTimelinePosition()
        {
            float centerSeconds = SongSeeker.SongTimeSeconds;
            float maxTime = SongSeeker.SongLengthSeconds;
            _leftTime = Mathf.Max(0, centerSeconds - _focusSeconds/2);
            _rightTime = Mathf.Min(maxTime, centerSeconds + _focusSeconds/2);
        }

        private void UpdateTimelineGridGraphics()
        {
            
        }

        private void UpdateTimelineEventGraphics()
        {
            
        }

    }
}