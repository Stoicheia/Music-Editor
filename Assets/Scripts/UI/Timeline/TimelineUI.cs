using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Input;
using Rhythm;
using RhythmEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
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
        public Transform RelativeTransform => _timelinePanel.transform;
        public Dictionary<RhythmEvent, EventNodeUI> EventToNode => _eventToNode;
        public Queue<EventNodeUI> EventNodePool => _eventNodePool;
        private bool _snapToGrid => Toolbar.SnapToGrid;
        
        [Header("User Settings")]
        [SerializeField] private float _focusSeconds;
        
        [SerializeField] private bool _ghostEventEnabled;
        [SerializeField] private float _clickCooldown = 0.3f;
        [SerializeField] private float _zoomSpeed = 0.3f;
        [SerializeField] [Range(0.1f, 3)] private float _minZoomSeconds = 1f;
        [SerializeField] [Range(10f, 120f)] private float _maxZoomSeconds = 1f;
        [SerializeField] [Range(0.001f, 1f)] private float _scrollSpeed = 1f;
        [SerializeField] [Range(0.1f, 100f)] private float _songScrollSpeed = 1f;
        [SerializeField] private bool _lockSeeker = true;
        [Header("Graphics")]
        [SerializeField] private Image _timelinePanel;
        [SerializeField] private GameObject _veryThinGridlinePrefab;
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
        [SerializeField] private float _generalDividerHeight = 250;
        [SerializeField] [Range(0, 1)] private float _thickDividerHeight = 250;
        [SerializeField] [Range(0, 1)] private float _thinDividerHeight = 250;
        [SerializeField] [Range(0, 1)] private float _veryThinDividerHeight = 250;
        [Header("Drawers")] 
        [SerializeField] private BarNumberDrawerUI _barNumberDrawer;

        [Header("Advanced")] 
        [SerializeField] private TimelineCommandManager _commandManager;
        [SerializeField] private TimelineFlagsDrawerUI _flagsDrawer;
        [SerializeField] private RectTransform _dividerGraphicsRoot;
        [SerializeField] private RectTransform _eventGraphicsRoot;
        [SerializeField] private RectTransform _connectorGraphicsRoot;
        [SerializeField] private RectTransform _foregroundGraphicsRoot;
        [SerializeField][Range(50, 1000)] private int _initialGraphicsCount = 100;

        private List<RectTransform> _veryThinGridlineObjects;
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
            _veryThinGridlineObjects = new List<RectTransform>();
            _eventObjects = new List<EventNodeUI>();
            _eventToNode = new Dictionary<RhythmEvent, EventNodeUI>();

            _commandManager.Clear();
            
            _barNumberDrawer.Init(this, Engine);
            _flagsDrawer.Init(this, Engine);
            
            for (int i = 0; i < _initialGraphicsCount; i++)
            {
                EventNodeUI eventInstance = Instantiate(_genericEventPrefab, _eventGraphicsRoot).GetComponent<EventNodeUI>();
                _eventObjects.Add(eventInstance);
                eventInstance.ReferenceTransform = _timelinePanel.rectTransform;
                eventInstance.ParentUI = this;
                eventInstance.Init();
            }
            
            CreateMoreGridlines(_initialGraphicsCount);

            _eventNodePool = new Queue<EventNodeUI>(_eventObjects);

            _ghostGraphic = Instantiate(_ghostEventPrefab, _eventGraphicsRoot).GetComponent<RectTransform>();
            _seekerGraphic = Instantiate(_seekerPrefab, _foregroundGraphicsRoot);
            _seekerGraphic.Audio = SongSeeker;
            _seekerGraphic.SetConfig(_seekerHeight, _seekerOffset);
            _seekerGraphic.Init(this, Engine);
            _seekerGraphic.OnMove += () => Toolbar.ToggleSeeker(false);
            
            foreach (var e in Engine.Events)
            {
                var eventNode = _eventNodePool.Dequeue();
                _eventToNode[e] = eventNode;
            }

            foreach (var enode in _eventObjects)
            {
                enode.OnClick += HandleClickEventNode;
                enode.OnRightClick += HandleRightClickEventNode;
                enode.OnMove += HandleMoveEventNode;
                enode.OnRequestExtension += HandleExtendEventNode;
                enode.OnPlace += HandlePlaceEventNode;
            }

            Toolbar.OnRequestBpmFlag += CreateBpmChangeFlag;
            Toolbar.OnRequestOffsetFlag += CreateOffsetFlag;
            Toolbar.OnRequestTimeSignatureFlag += CreateTimeSignatureChangeFlag;
            Toolbar.OnToggleSeekerState += ToggleSeekerLock;
            Toolbar.OnToggleSnapState += ToggleSnapState;

            RecalculateSubdivisions();
            _leftTime = Mathf.Min(SongSeeker.SongLengthSeconds - _focusSeconds, SongSeeker.SongTimeSeconds - _focusSeconds / 2);
            _rightTime = Mathf.Min(SongSeeker.SongLengthSeconds, SongSeeker.SongTimeSeconds + _focusSeconds / 2);
            
            _isInitialised = true;
        }

        private void CreateMoreGridlines(int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                RectTransform veryThinLineInstance = 
                    Instantiate(_veryThinGridlinePrefab, _dividerGraphicsRoot).GetComponent<RectTransform>();
                _veryThinGridlineObjects.Add(veryThinLineInstance);
                RectTransform thinLineInstance =
                    Instantiate(_thinGridlinePrefab, _dividerGraphicsRoot).GetComponent<RectTransform>();
                _thinGridlineObjects.Add(thinLineInstance);
                RectTransform thickLineInstance = Instantiate(_thickGridlinePrefab, _dividerGraphicsRoot)
                    .GetComponent<RectTransform>();
                _thickGridlineObjects.Add(thickLineInstance);
            }
        }

        public void Clear()
        {
            if (!_isInitialised) return;
            _flagsDrawer.Clear();
            _seekerGraphic.Clear();
            _barNumberDrawer.Clear();
            foreach (var graphic in _eventObjects)
            {
                graphic.DestroyChildren();
                Destroy(graphic.gameObject);
            }
            _eventObjects.Clear();
            _eventNodePool.Clear();
            _eventToNode.Clear();
            foreach (var graphic in _thinGridlineObjects)
            {
                Destroy(graphic.gameObject);
            }
            _thinGridlineObjects.Clear();
            foreach (var graphic in _thickGridlineObjects)
            {
                Destroy(graphic.gameObject);
            }
            _thickGridlineObjects.Clear();
            Destroy(_seekerGraphic.gameObject);
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

            if (Keybinds.Shift)
            {
                _focusSeconds *= (1 - _zoomSpeed * Input.mouseScrollDelta.y);
                _focusSeconds = Mathf.Clamp(_focusSeconds, _minZoomSeconds, _maxZoomSeconds);
                if (!_lockSeeker)
                {
                    float center = (_leftTime + _rightTime) / 2;
                    _leftTime = center - _focusSeconds / 2;
                    _rightTime = center + _focusSeconds / 2;
                }
            }

            else
            {
                float moveBy = -_scrollSpeed * Input.mouseScrollDelta.y * _focusSeconds;
                if (_rightTime + moveBy > 0)
                {
                    if (!_lockSeeker)
                    {
                        _leftTime += moveBy;
                        _rightTime += moveBy;
                    }
                    else
                    {
                        SongSeeker.Scroll(moveBy);
                    }
                }
            }
        }
        
        private EventNodeUI PlaceNew(float time, float vertical)
        {
            PlaceEventCommand eventCommand = new PlaceEventCommand(time, vertical);
            _commandManager.ApplyCommand(eventCommand, this, Engine);
            return eventCommand.EventNode;
        }

        private void DeleteNode(EventNodeUI node)
        {
            DeleteNodeCommand deleteCommand = new DeleteNodeCommand(node);
            _commandManager.ApplyCommand(deleteCommand, this, Engine);
        }
        
        public void ClearAllEvents()
        {
            Debug.Log("The ability to clear all events has been removed.");
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

            _seekerGraphic.Draw(Engine, _panel, _leftTime, _rightTime, 0);
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
                    bbs = nextBpmChange.Lock ? bbs.NewBar() : bbs.NextBar();
                    nextBpmChange = bpmIndex < bpmChanges.Count - 1 ? bpmChanges[++bpmIndex] : null;
                }

               
                // Update time in terms of beats
                float subdivUnit = (float) 1 / Toolbar.Subdivision;
                int order = bbs.Beat == 1 && bbs.Subdiv == 0 ? 1 : bbs.Subdiv == 0 ? 0 : -1;

                _subdivisionsAndOrders.Add((t, order, bbs.Bar));
                t += MathUtility.BeatsToSeconds(1, bpm)/Toolbar.Subdivision;
                bbs = bbs.Add(0,0, subdivUnit);
            }
        }

        private void UpdateTimelineGridGraphics(int fromIndex)
        {
            var subdivIndex = Math.Max(0, fromIndex - 1);

            // Draw the gridlines within the focus range
            int veryThinLineIndex = 0;
            int thinLineIndex = 0;
            int thickLineIndex = 0;

            while (subdivIndex < _subdivisionsAndOrders.Count && _subdivisionsAndOrders[subdivIndex].Item1 < _rightTime)
            {
                float t = MathUtility.InverseLerpUnclamped(_leftTime, _rightTime, _subdivisionsAndOrders[subdivIndex].Item1);
                Vector2 timelinePos = Vector2.LerpUnclamped(_panelLeft, _panelRight, t);
                RectTransform lineObj;
                float relativeHeight;

                if (veryThinLineIndex >= _thinGridlineObjects.Count - 1 || thinLineIndex >= _thickGridlineObjects.Count - 1)
                {
                    CreateMoreGridlines(_initialGraphicsCount);
                }

                (lineObj, relativeHeight) = _subdivisionsAndOrders[subdivIndex].Item2 switch
                {
                    -1 => (_veryThinGridlineObjects[veryThinLineIndex++], _veryThinDividerHeight),
                    0 => (_thinGridlineObjects[thinLineIndex++], _thinDividerHeight),
                    1 => (_thickGridlineObjects[thickLineIndex++], _thickDividerHeight),
                    _ => throw new ArgumentOutOfRangeException()
                };

                lineObj.anchoredPosition = timelinePos;
                lineObj.gameObject.SetActive(true);
                lineObj.sizeDelta = new Vector2(lineObj.sizeDelta.x, relativeHeight * _generalDividerHeight);
                lineObj.anchoredPosition += Vector2.up * (_generalDividerHeight * (1 - relativeHeight)) / 2;

                subdivIndex++;
            }

            // Disable the rest of the gridlines
            for (int i = veryThinLineIndex; i < _veryThinGridlineObjects.Count; i++)
            {
                _veryThinGridlineObjects[i].gameObject.SetActive(false);
            }
            
            for (int i = thinLineIndex; i < _thinGridlineObjects.Count; i++)
            {
                _thinGridlineObjects[i].gameObject.SetActive(false);
            }

            for (int i = thickLineIndex; i < _thickGridlineObjects.Count; i++)
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

                float vertical = e.Vertical;
                
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

            var (songTime, valid) = MathUtility.PositionToTime(
                Selector.MouseScreenPosition,
                _timelinePanel.transform,
                _panel,
                _leftTime,
                _rightTime,
                _snapToGrid ? Snap : null,
                true,
                false
            );
            
            if (!valid || _activeNode != null)
            {
                _ghostGraphic.gameObject.SetActive(false);
                _ghostTime = -1000;
                return;
            }
            _ghostGraphic.gameObject.SetActive(true);
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
        
        private void ToggleSnapState((bool, int) eData)
        {
            // behaviour moved to Toolbar.cs
        }

#region NODE FLAG AND MOUSE EVENTS
        private MoveEventCommand _queuedMoveCommand;
        private void HandleClickEventNode(EventNodeUI node)
        {
            if (Toolbar.ActiveOption != ToolbarOption.Select) return;

            MoveEventCommand moveCommand = new MoveEventCommand(node, node.Time, node.Event.DurationSeconds, node.Vertical);
            _queuedMoveCommand = moveCommand;
        }

        private void HandleRightClickEventNode(EventNodeUI node)
        {
            if (Toolbar.ActiveOption != ToolbarOption.Draw) return;
        
            DeleteNode(node);
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
            node.Event.Vertical = node.Vertical;
            
            Engine.UpdateEvents();
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
        
        private void HandlePlaceEventNode(EventNodeUI obj)
        {
            if (_queuedMoveCommand != null)
            {
                _commandManager.ApplyCommand(_queuedMoveCommand, this, Engine);
                _queuedMoveCommand = null;
            }
        }
        
        private void CreateBpmChangeFlag()
        {
            Engine.BpmChanges.Add(new BpmChange(SongSeeker.SongTimeSeconds, Bpm));
            Engine.ForceUpdate();
        }
        
        private void CreateOffsetFlag()
        {
            Engine.BpmChanges.Add(new BpmChange(SongSeeker.SongTimeSeconds, Bpm, true));
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
                _activeNode.Event.Vertical = vertical;
                _clickTimer = _clickCooldown;
            }
        }

        public void Move(SelectInfo info, Vector2 pos)
        {
            if (!_isInitialised) return;
            if (Toolbar.ActiveOption != ToolbarOption.Draw)
            {
                _seekerGraphic.MoveTo(pos);
                return;
            }
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
#endregion
    }
}