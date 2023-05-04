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
        [SerializeField] private float _zoomSpeed = 0.3f;
        [SerializeField] [Range(0.1f, 3)] private float _minZoomSeconds = 1f;
        [SerializeField] [Range(10f, 120f)] private float _maxZoomSeconds = 1f;
        [SerializeField] [Range(0.1f, 100f)] private float _scrollSpeed = 1f;
        [SerializeField] [Range(0.1f, 100f)] private float _songScrollSpeed = 1f;
        [SerializeField] private bool _lockSeeker = true;
        [Header("Graphics")]
        [SerializeField] private Image _timelinePanel;
        [SerializeField] private GameObject _thinGridlinePrefab;
        [SerializeField] private GameObject _thickGridlinePrefab;
        [SerializeField] private TimelineSeeker _seekerPrefab;
        [SerializeField] private EventNodeUI _genericEventPrefab;
        [SerializeField] private GameObject _ghostEventPrefab;
        [Header("Graphics Options")]
        [SerializeField] private float _seekerOffset = 100;
        [SerializeField] private float _seekerHeight = 100;
        [SerializeField] private float _nodeRadius = 100;
        [SerializeField] private float _extenderHeight = 50;
        [SerializeField] private float _dividerHeight = 250;
        [Header("Drawers")] 
        [SerializeField] private BarNumberDrawerUI _barNumberDrawer;
        [Header("Advanced")] 
        [SerializeField] private TimelineFlagsDrawerUI _flagsDrawer;
        [SerializeField] private RectTransform _dividerGraphicsRoot;
        [SerializeField] private RectTransform _eventGraphicsRoot;
        [SerializeField] private RectTransform _connectorGraphicsRoot;
        [SerializeField] private RectTransform _foregroundGraphicsRoot;
        [SerializeField][Range(50, 1000)] private int _initialGraphicsCount = 100;

        private List<RectTransform> _thinGridlineObjects;
        private List<RectTransform> _thickGridlineObjects;
        private List<EventNodeUI> _eventObjects;
        private RectTransform _ghostGraphic;
        private TimelineSeeker _seekerGraphic;

        private float _ghostTime;

        private float _leftTime;
        private float _rightTime;
        private List<(float, int, int)> _subdivisionsAndOrders;

        private Rect _panel;
        private Vector2 _panelLeft;
        private Vector2 _panelRight;

        private float _clickTimer;

        private Dictionary<RhythmEvent, EventNodeUI> _eventToNode;
        private Queue<EventNodeUI> _eventNodePool;

        private float Offset => 0;
        private float Bpm => Engine.GetBpm(SongSeeker.SongTimeSeconds);
        private TimeSignature TimeSignature => Engine.GetTimeSignature(SongSeeker.SongTimeSeconds);

        private bool _isInitialised;

        private void Awake()
        {
            EventNodeUI.ExtenderRoot = _connectorGraphicsRoot;
            _flagsDrawer.Timeline = this;
        }

        public void Init()
        {
            _subdivisionsAndOrders = new List<(float, int, int)>();
            _thickGridlineObjects = new List<RectTransform>();
            _thinGridlineObjects = new List<RectTransform>();
            _eventObjects = new List<EventNodeUI>();
            _eventToNode = new Dictionary<RhythmEvent, EventNodeUI>();
            
            _barNumberDrawer.Init(this, Engine);
            _flagsDrawer.Init(this, Engine);
            
            for (int i = 0; i < _initialGraphicsCount; i++)
            {
                RectTransform thinLineInstance = Instantiate(_thinGridlinePrefab, _dividerGraphicsRoot).GetComponent<RectTransform>();
                _thinGridlineObjects.Add(thinLineInstance);
                RectTransform thickLineInstance = Instantiate(_thickGridlinePrefab, _dividerGraphicsRoot).GetComponent<RectTransform>();
                _thickGridlineObjects.Add(thickLineInstance);

                EventNodeUI eventInstance = Instantiate(_genericEventPrefab, _eventGraphicsRoot).GetComponent<EventNodeUI>();
                _eventObjects.Add(eventInstance);
                eventInstance.ReferenceTransform = _timelinePanel.rectTransform;
                eventInstance.ParentUI = this;
            }

            _eventNodePool = new Queue<EventNodeUI>(_eventObjects);

            _ghostGraphic = Instantiate(_ghostEventPrefab, _eventGraphicsRoot).GetComponent<RectTransform>();
            _seekerGraphic = Instantiate(_seekerPrefab, _foregroundGraphicsRoot);
            _seekerGraphic.Audio = SongSeeker;

            foreach (var enode in _eventObjects)
            {
                enode.OnClick += HandleClickEventNode;
                enode.OnRightClick += HandleRightClickEventNode;
                enode.OnMove += HandleMoveEventNode;
                enode.OnRequestExtension += HandleExtendEventNode;
            }

            Toolbar.OnRequestBpmFlag += CreateBpmChangeFlag;
            Toolbar.OnRequestTimeSignatureFlag += CreateTimeSignatureChangeFlag;
            Toolbar.OnToggleSeekerState += ToggleSeekerLock;

            RecalculateSubdivisions();
            _isInitialised = true;
        }


        private void Update()
        {
            if (!_isInitialised) return;
            _panel = _timelinePanel.rectTransform.rect;
            _panelLeft = new Vector2(_panel.xMin, _panel.center.y);
            _panelRight = new Vector2(_panel.xMax, _panel.center.y);
            
            // Find the start of the focus range
            int subdivIndex = 0;
            while (subdivIndex < _subdivisionsAndOrders.Count && _subdivisionsAndOrders[subdivIndex].Item1 < _leftTime)
            {
                subdivIndex++;
            }
            
            RecalculateSubdivisions();
            // Update graphics
            UpdateTimelinePosition();
            UpdateTimelineGridGraphics(subdivIndex);
            _barNumberDrawer.UpdateSubdivisions(_subdivisionsAndOrders);
            _barNumberDrawer.Draw(Engine, _panel, _leftTime, _rightTime, subdivIndex);
            UpdateTimelineEventGraphics();
            
            if(_flagsDrawer != null)
                _flagsDrawer.Draw(Engine, _panel, _leftTime, _rightTime);
            if(_ghostEventEnabled && Toolbar.ActiveOption == ToolbarOption.Draw) UpdateGhostGraphics();
            else _ghostGraphic.gameObject.SetActive(false);

            // Update state and read input
            SongSeeker.ScrollSpeedMultiplier = _focusSeconds;

            _clickTimer -= Time.deltaTime;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                _focusSeconds *= (1 - _zoomSpeed * Input.mouseScrollDelta.y);
                _focusSeconds = Mathf.Clamp(_focusSeconds, _minZoomSeconds, _maxZoomSeconds);
            }

            else
            {
                float moveBy = -_scrollSpeed * Input.mouseScrollDelta.y;
                if (!_lockSeeker && _rightTime + moveBy > 0)
                {
                    _leftTime += moveBy;
                    _rightTime += moveBy;
                }
            }
        }
        
        private EventNodeUI PlaceNew(float time, float vertical)
        {
            RhythmEvent newEvent = new RhythmEvent(time);
            Engine.AddEvent(newEvent);
            _eventToNode[newEvent] = _eventNodePool.Dequeue();
            _eventToNode[newEvent].Vertical = vertical;
            _eventToNode[newEvent].Event = newEvent;
            return _eventToNode[newEvent];
        }
        
        public void ClearAllEvents()
        {
            foreach (var node in _eventToNode.Values)
            {
                _eventNodePool.Enqueue(node);
            }
            
            _eventToNode.Clear();
            Engine.ClearLevelData();
        }

        private void UpdateTimelinePosition()
        {
            float centerSeconds = SongSeeker.SongTimeSeconds;
            float maxTime = SongSeeker.SongLengthSeconds;

            if (_lockSeeker)
            {
                _leftTime = Mathf.Min(maxTime - _focusSeconds, centerSeconds - _focusSeconds / 2);
                _rightTime = Mathf.Min(maxTime, centerSeconds + _focusSeconds / 2);
            }

            _seekerGraphic.Draw( _panel, _leftTime, _rightTime, centerSeconds, _seekerHeight, _seekerOffset);
        }

        private void RecalculateSubdivisions()
        {
            _subdivisionsAndOrders.Clear();
            var bpmChanges = Engine.BpmChanges;
            var timeSigChanges = Engine.TimeSigChanges;

            int bpmIndex = 0;
            int timeSigIndex = 0;
            float bpm = bpmChanges[bpmIndex++].Bpm;
            Rhythm.TimeSignature timeSig = timeSigChanges[timeSigIndex++].TimeSignature;

            BpmChange nextBpmChange = bpmIndex < bpmChanges.Count ? bpmChanges[bpmIndex] : null;
            TimeSignatureChange nextSigChange = timeSigIndex < timeSigChanges.Count ? timeSigChanges[timeSigIndex] : null;

            float t = Offset;
            
            BarBeatSubdiv bbs = BarBeatSubdiv.Beginning(timeSig.BeatsInABar());
            
            while (t < SongSeeker.SongLengthSeconds)
            {
                if (nextSigChange != null && t >= nextSigChange.Time)
                {
                    t = nextSigChange.Time;
                    timeSig = nextSigChange.TimeSignature;
                    nextSigChange = timeSigIndex < timeSigChanges.Count - 1 ? timeSigChanges[++timeSigIndex] : null;
                    bbs = bbs.ChangeBeatsPerBar(timeSig.BeatsInABar());
                    bbs = bbs.NextBar();
                }
                
                if (nextBpmChange != null && t >= nextBpmChange.Time)
                {
                    t = nextBpmChange.Time;
                    bpm = nextBpmChange.Bpm;
                    nextBpmChange = bpmIndex < bpmChanges.Count - 1 ? bpmChanges[++bpmIndex] : null;
                    bbs = bbs.NextBar();
                }

               
                // Update time in terms of beats
                float subdivUnit = (float) 1 / Toolbar.Subdivision;
                int order = bbs.Beat == 1 ? 1 : bbs.Subdiv <= 0.001f ? 0 : -1;

                _subdivisionsAndOrders.Add((t, order, bbs.Bar));
                t += MathUtility.BeatsToSeconds(1, bpm)/Toolbar.Subdivision;
                bbs = bbs.Add(0,0, subdivUnit);
            }
        }

        private void UpdateTimelineGridGraphics(int fromIndex)
        {
            var subdivIndex = Math.Max(0, fromIndex - 1);

            // Draw the gridlines within the focus range
            int thinLineIndex = 0;
            int thickLineIndex = 0;
            
            while (subdivIndex < _subdivisionsAndOrders.Count && _subdivisionsAndOrders[subdivIndex].Item1 < _rightTime)
            {
                float t = MathUtility.InverseLerpUnclamped(_leftTime, _rightTime, _subdivisionsAndOrders[subdivIndex].Item1);
                Vector2 timelinePos = Vector2.LerpUnclamped(_panelLeft, _panelRight, t);
                RectTransform lineObj;
                if (_subdivisionsAndOrders[subdivIndex].Item2 <= 0)
                {
                    lineObj = _thinGridlineObjects[thinLineIndex++];
                }
                else
                {
                    lineObj = _thickGridlineObjects[thickLineIndex++];
                }
                
                lineObj.anchoredPosition = timelinePos;
                lineObj.gameObject.SetActive(true);
                lineObj.sizeDelta = new Vector2(lineObj.sizeDelta.x, _dividerHeight);

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
                var eventGraphic = _eventToNode[e];
                
                if (!e.WithinRange(_leftTime - 1, _rightTime + 1))
                {
                    eventGraphic.DisableGraphics();
                    continue;
                }

                float vertical = eventGraphic.Vertical;
                
                eventGraphic.Event = e;
                eventGraphic.Draw(_panel, _leftTime, _rightTime, vertical);
                eventGraphic.gameObject.SetActive(true);

                eventGraphic.rectTransform.sizeDelta = new Vector2(_nodeRadius * 2, _nodeRadius * 2);
                eventGraphic.ExtenderHeight = _extenderHeight;
                eventGraphic.NodeRadius = _nodeRadius;
            }

            // Disable unused event graphics
            foreach(var e in _eventNodePool)
            {
                e.DisableGraphics();
            }
        }
        
        private void UpdateGhostGraphics()
        {
            Vector2 anchoredMousePos = _timelinePanel.transform.InverseTransformPoint(Selector.MouseScreenPosition);
            if (!_panel.Contains(anchoredMousePos) || _activeNode != null)
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
            float y = anchoredMousePos.y;
            
            Vector2 newPos = new Vector2(x, y);
            _ghostGraphic.anchoredPosition = newPos;
            _ghostGraphic.sizeDelta = new Vector2(_nodeRadius * 2, _nodeRadius * 2);
        }

        private float Snap(float time)
        {
            // Binary search to find nearest time on the beat
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
        
        private void ToggleSeekerLock(bool @lock)
        {
            _lockSeeker = @lock;
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
            node.Vertical = Mathf.InverseLerp(_panel.yMin, _panel.yMax, anchoredPos.y);
            
            Engine.UpdateEvents();
        }
        
        private void CreateBpmChangeFlag()
        {
            Engine.BpmChanges.Add(new BpmChange(SongSeeker.SongTimeSeconds, Bpm));
            Engine.ForceUpdate();
        }

        private void CreateTimeSignatureChangeFlag()
        {
            Engine.TimeSigChanges.Add(new TimeSignatureChange(SongSeeker.SongTimeSeconds, TimeSignature));
            Engine.ForceUpdate();
        }

        public void Select(SelectInfo info, Vector2 pos, bool empty = false)
        {
            
        }

        private EventNodeUI _activeNode;
        public void Click(SelectInfo info, Vector2 pos)
        {
            if (Toolbar.ActiveOption != ToolbarOption.Draw) return;
            
            Vector2 anchoredPos = _timelinePanel.transform.InverseTransformPoint(pos);

            if (_clickTimer < 0 && anchoredPos.x > _panelLeft.x && anchoredPos.x < _panelRight.x)
            {
                float relPos = Mathf.InverseLerp(_panelLeft.x, _panelRight.x, anchoredPos.x);
                float time = Mathf.Lerp(_leftTime, _rightTime, relPos);
                float vertical = Mathf.InverseLerp(_panel.yMin, _panel.yMax, anchoredPos.y);
                _activeNode = PlaceNew(_snapToGrid ? Snap(time) : time, vertical);
                _clickTimer = _clickCooldown;
            }
        }

        public void Move(SelectInfo info, Vector2 pos)
        {
            if (Toolbar.ActiveOption != ToolbarOption.Draw) return;
            if (_activeNode == null) return;
            
            HandleExtendEventNode(_activeNode, pos);
        }

        public void Place(SelectInfo info, Vector2 pos)
        {
            _activeNode = null;
        }

        public void RightClicked(SelectInfo info, Vector2 pos)
        {
            
        }

        public void HandleExtendEventNode(EventNodeUI node, Vector2 pos)
        {
            Vector2 anchoredPos = _timelinePanel.transform.InverseTransformPoint(pos);
            
            if (!_panel.Contains(anchoredPos)) return;
            
            float relPos = Mathf.InverseLerp(_panelLeft.x, _panelRight.x, anchoredPos.x);
            float time = Mathf.Lerp(_leftTime, _rightTime, relPos);
            float boundedTime = Mathf.Max(node.Event.TimeSeconds, time);
            node.Event.SetEndTime(_snapToGrid ? Snap(boundedTime) : boundedTime);
        }
    }
}