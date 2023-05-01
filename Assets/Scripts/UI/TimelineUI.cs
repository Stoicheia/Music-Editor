using System;
using System.Collections.Generic;
using System.Linq;
using Rhythm;
using RhythmEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using StringUtility = Utility.StringUtility;

namespace UI
{
    public class TimelineUI : MonoBehaviour, ISelectorInteractor
    {
        public SelectorUI Selector { get; set; }
        public SongSeekerUI SongSeeker { get; set; }
        public LevelEditorUI LevelEditor { get; set; }
        public EditorEngine Engine { get; set; }
        public ToolbarUI Toolbar { get; set; }
        
        [Header("User Settings")]
        [SerializeField] private float _focusSeconds;
        [SerializeField] private bool _snapToGrid;
        [SerializeField] private bool _ghostEventEnabled;
        [SerializeField] private float _clickCooldown = 0.3f;
        [Header("Graphics")]
        [SerializeField] private Image _timelinePanel;
        [SerializeField] private GameObject _thinGridlinePrefab;
        [SerializeField] private GameObject _thickGridlinePrefab;
        [SerializeField] private GameObject _seekerPrefab;
        [SerializeField] private EventNodeUI _genericEventPrefab;
        [SerializeField] private GameObject _ghostEventPrefab;
        [Header("Graphics Options")] 
        [SerializeField] private float _stackThreshold = 0.001f;
        [SerializeField] [Range(1, 10)] private int _stackLimit = 4;
        [SerializeField] private float _stackExpand = 100;
        [SerializeField] private float _seekerOffset = 100;
        [Header("Advanced")] 
        [SerializeField] private RectTransform _dividerGraphicsRoot;
        [SerializeField] private RectTransform _eventGraphicsRoot;
        [SerializeField] private RectTransform _foregroundGraphicsRoot;
        [SerializeField][Range(50, 1000)] private int _initialGraphicsCount = 100;

        private List<RectTransform> _thinGridlineObjects;
        private List<RectTransform> _thickGridlineObjects;
        private List<EventNodeUI> _eventObjects;
        private RectTransform _ghostGraphic;
        private RectTransform _seekerGraphic;

        private float _ghostTime;

        private float _leftTime;
        private float _rightTime;
        private List<(float, int)> _subdivisionsAndOrders;

        private Rect _panel;
        private Vector2 _panelLeft;
        private Vector2 _panelRight;

        private float _clickTimer;

        private Dictionary<RhythmEvent, EventNodeUI> _eventToNode;
        private Queue<EventNodeUI> _eventNodePool;

        private float Offset => 0;
        private float Bpm => LevelEditor.CurrentBpm;
        private TimeSignature TimeSignature => LevelEditor.CurrentTimeSignature;

        private bool _isInitialised;

        public void Init()
        {
            _subdivisionsAndOrders = new List<(float, int)>();
            _thickGridlineObjects = new List<RectTransform>();
            _thinGridlineObjects = new List<RectTransform>();
            _eventObjects = new List<EventNodeUI>();
            _eventToNode = new Dictionary<RhythmEvent, EventNodeUI>();
            
            for (int i = 0; i < _initialGraphicsCount; i++)
            {
                RectTransform thinLineInstance = Instantiate(_thinGridlinePrefab, _dividerGraphicsRoot).GetComponent<RectTransform>();
                _thinGridlineObjects.Add(thinLineInstance);
                RectTransform thickLineInstance = Instantiate(_thickGridlinePrefab, _dividerGraphicsRoot).GetComponent<RectTransform>();
                _thickGridlineObjects.Add(thickLineInstance);
                EventNodeUI eventInstance = Instantiate(_genericEventPrefab, _eventGraphicsRoot).GetComponent<EventNodeUI>();
                _eventObjects.Add(eventInstance);
            }

            _eventNodePool = new Queue<EventNodeUI>(_eventObjects);

            _ghostGraphic = Instantiate(_ghostEventPrefab, _eventGraphicsRoot).GetComponent<RectTransform>();
            _seekerGraphic = Instantiate(_seekerPrefab, _foregroundGraphicsRoot).GetComponent<RectTransform>();

            foreach (var enode in _eventObjects)
            {
                enode.OnClick += HandleClickEventNode;
                enode.OnRightClick += HandleRightClickEventNode;
                enode.OnMove += HandleMoveEventNode;
            }
            
            RecalculateSubdivisions();
            _isInitialised = true;
        }

        private void Update()
        {
            if (!_isInitialised) return;
            _panel = _timelinePanel.rectTransform.rect;
            _panelLeft = new Vector2(_panel.xMin, _panel.center.y);
            _panelRight = new Vector2(_panel.xMax, _panel.center.y);
            
            UpdateTimelinePosition();
            UpdateTimelineGridGraphics();
            UpdateTimelineEventGraphics();
            if(_ghostEventEnabled && Toolbar.ActiveOption == ToolbarOption.Draw) UpdateGhostGraphics();
            else _ghostGraphic.gameObject.SetActive(false);

            _clickTimer -= Time.deltaTime;
        }
        
        public void PlaceNew(float time)
        {
            RhythmEvent newEvent = new RhythmEvent(time);
            Engine.AddEvent(newEvent);
            _eventToNode[newEvent] = _eventNodePool.Dequeue();
        }

        private void UpdateGhostGraphics()
        {
            Vector2 anchoredMousePos = _timelinePanel.transform.InverseTransformPoint(Selector.MouseScreenPosition);
            if (!_panel.Contains(anchoredMousePos))
            {
                _ghostGraphic.gameObject.SetActive(false);
                _ghostTime = -1000;
                return;
            }
            _ghostGraphic.gameObject.SetActive(true);
            float relPos = Mathf.InverseLerp(_panelLeft.x, _panelRight.x, anchoredMousePos.x);
            float songTime = Mathf.Lerp(_leftTime, _rightTime, relPos);
            if (_snapToGrid) songTime = Snap(songTime);
            _ghostTime = songTime;
            float newRelPos = Mathf.InverseLerp(_leftTime, _rightTime, songTime);

            float x = Vector2.Lerp(_panelLeft, _panelRight, newRelPos).x;

            int stacks = 1 + Engine.Events.Count(x => Mathf.Abs(x.TimeSeconds - _ghostTime) <= _stackThreshold);
            float y = Mathf.LerpUnclamped(_panel.yMin - _stackExpand, _panel.yMax + _stackExpand, (float)stacks/(stacks + 1));
            Vector2 newPos = new Vector2(x, y);
            _ghostGraphic.anchoredPosition = newPos;
        }

        private void UpdateTimelinePosition()
        {
            float centerSeconds = SongSeeker.SongTimeSeconds;
            float maxTime = SongSeeker.SongLengthSeconds;
            _leftTime = Mathf.Min(maxTime - _focusSeconds, centerSeconds - _focusSeconds/2);
            _rightTime = Mathf.Min(maxTime, centerSeconds + _focusSeconds/2);

            float t = Mathf.InverseLerp(_leftTime, _rightTime, centerSeconds);
            _seekerGraphic.anchoredPosition = Vector2.Lerp(_panelLeft, _panelRight, t) + Vector2.up * _seekerOffset;
        }

        private void RecalculateSubdivisions()
        {
            _subdivisionsAndOrders.Clear();

            float t = Offset;
            int beatCount = 0;
            while (t < SongSeeker.SongLengthSeconds)
            {
                _subdivisionsAndOrders.Add((t, beatCount == 0 ? 1 : 0));
                t += MathUtility.BeatsToSeconds(1, Bpm);
                beatCount++;
                beatCount %= TimeSignature.BeatsInABar();
            }
        }

        private void UpdateTimelineGridGraphics()
        {
            // Find the start of the focus range
            int subdivIndex = 0;
            while (subdivIndex < _subdivisionsAndOrders.Count && _subdivisionsAndOrders[subdivIndex].Item1 < _leftTime)
            {
                subdivIndex++;
            }

            // Draw the gridlines within the focus range
            int thinLineIndex = 0;
            int thickLineIndex = 0;
            
            while (subdivIndex < _subdivisionsAndOrders.Count && _subdivisionsAndOrders[subdivIndex].Item1 < _rightTime)
            {
                float t = Mathf.InverseLerp(_leftTime, _rightTime, _subdivisionsAndOrders[subdivIndex].Item1);
                Vector2 timelinePos = Vector2.Lerp(_panelLeft, _panelRight, t);
                if (_subdivisionsAndOrders[subdivIndex].Item2 <= 0)
                {
                    var lineObj = _thinGridlineObjects[thinLineIndex++];
                    lineObj.anchoredPosition = timelinePos;
                    lineObj.gameObject.SetActive(true);
                }
                else
                {
                    var lineObj = _thickGridlineObjects[thickLineIndex++];
                    lineObj.anchoredPosition = timelinePos;
                    lineObj.gameObject.SetActive(true);                
                }

                subdivIndex++;
            }

            // Disable the rest of the gridlines
            for (int i = thinLineIndex; i < _initialGraphicsCount; i++)
            {
                _thinGridlineObjects[i].gameObject.SetActive(false);
            }

            for (int i = thickLineIndex; i < _initialGraphicsCount; i++)
            {
                _thickGridlineObjects[i].gameObject.SetActive(false);
            }
        }

        private void UpdateTimelineEventGraphics()
        {
            var events = Engine.Events; // assume sorted by time

            // Place event graphics
            for (int i = 0; i < events.Count; i++)
            {
                RhythmEvent e = events[i];
                if (e.TimeSeconds < _leftTime || e.TimeSeconds > _rightTime) continue;
                var eventGraphic = _eventToNode[e];
                float t = Mathf.InverseLerp(_leftTime, _rightTime, e.TimeSeconds);
                float x = Vector2.Lerp(_panelLeft, _panelRight, t).x;
                
                // Calculate stack from other events at same time
                int stacksBeforeMe = 0;
                for (int j = i-1; j >= 0 && Mathf.Abs(events[j].TimeSeconds - e.TimeSeconds) <= _stackThreshold; j--)
                {
                    stacksBeforeMe++;
                }

                int stacksAfterMe = 0;
                for (int j = i+1; j < events.Count && Mathf.Abs(events[j].TimeSeconds - e.TimeSeconds) <= _stackThreshold; j++)
                {
                    stacksAfterMe++;
                }

                if (Mathf.Abs(_ghostTime - e.TimeSeconds) <= _stackThreshold) stacksAfterMe++;
                
                int stackOrder = stacksBeforeMe + 1;
                int stackTotal = stackOrder + stacksAfterMe;

                float y = Mathf.LerpUnclamped(_panel.yMin - _stackExpand, _panel.yMax + _stackExpand, (float)stackOrder/(stackTotal + 1));
                eventGraphic.rectTransform.anchoredPosition = new Vector2(x, y);
                eventGraphic.gameObject.SetActive(true);

                eventGraphic.Event = e;
            }

            // Disable unused event graphics
            foreach(var e in _eventNodePool)
            {
                e.gameObject.SetActive(false);
            }
        }

        private float Snap(float time)
        {
            // binary search
            var times = _subdivisionsAndOrders.Select(x => x.Item1).ToList();
            int index = 0;
            for (int k = times.Count / 2; k > 0; k /= 2)
            {
                while (index + k < times.Count && times[index + k] < time)
                {
                    index += k;
                }
            }

            var prevTime = _subdivisionsAndOrders[index % _subdivisionsAndOrders.Count].Item1;
            var nextTime = _subdivisionsAndOrders[(index + 1) % _subdivisionsAndOrders.Count].Item1;
            return Mathf.Abs(time - prevTime) < Mathf.Abs(time - nextTime) ? prevTime : nextTime;
        }

        private void HandleClickEventNode(EventNodeUI node)
        {
            
        }

        private void HandleRightClickEventNode(EventNodeUI node)
        {
            if (Toolbar.ActiveOption != ToolbarOption.Draw) return;
        
            _eventNodePool.Enqueue(node);
            _eventToNode.Remove(node.Event);
            Engine.RemoveEvent(node.Event);
        }

        private void HandleMoveEventNode(EventNodeUI node, Vector2 pos)
        {
            if (Toolbar.ActiveOption != ToolbarOption.Select) return;

            Vector2 anchoredPos = _timelinePanel.transform.InverseTransformPoint(pos);

            if (!_panel.Contains(anchoredPos)) return;
            
            float relPos = Mathf.InverseLerp(_panelLeft.x, _panelRight.x, anchoredPos.x);
            float time = Mathf.Lerp(_leftTime, _rightTime, relPos);
            node.Event.SetTime(_snapToGrid ? Snap(time) : time);
            
            Engine.UpdateEvents();
        }

        public void ClearAllEvents()
        {
            Engine.Events.Clear();
        }

        public void Select(SelectInfo info, Vector2 pos, bool empty = false)
        {
            if (Toolbar.ActiveOption != ToolbarOption.Draw) return;
            
            Vector2 anchoredPos = _timelinePanel.transform.InverseTransformPoint(pos);

            if (_clickTimer < 0 && anchoredPos.x > _panelLeft.x && anchoredPos.x < _panelRight.x)
            {
                float relPos = Mathf.InverseLerp(_panelLeft.x, _panelRight.x, anchoredPos.x);
                float time = Mathf.Lerp(_leftTime, _rightTime, relPos);
                PlaceNew(_snapToGrid ? Snap(time) : time);
                _clickTimer = _clickCooldown;
            }
        }

        public void Click(SelectInfo info, Vector2 pos)
        {
            
        }

        public void Move(SelectInfo info, Vector2 pos)
        {
            
        }

        public void Place(SelectInfo info, Vector2 pos)
        {
            
        }

        public void RightClicked(SelectInfo info, Vector2 pos)
        {
            
        }
    }
}