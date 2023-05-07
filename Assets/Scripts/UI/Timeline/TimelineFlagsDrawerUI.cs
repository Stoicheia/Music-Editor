using System;
using System.Collections.Generic;
using System.Linq;
using RhythmEngine;
using UnityEngine;
using Utility;

namespace UI
{
    public class TimelineFlagsDrawerUI : TimelineDrawerHelperUI
    {
        private const float DRAW_PADDING_SECONDS = 1;
        public override void Init(TimelineUI timeline, EditorEngine data)
        {
            base.Init(timeline, data);
            
            for (int i = 0; i < _initialisedFields; i++)
            {
                BpmField newBpmField = Instantiate(_bpmFieldPrefab, _graphicsRoot);
                TimeSignatureField newSigField = Instantiate(_timeSignatureFieldPrefab, _graphicsRoot);
                
                _bpmFields.Add(newBpmField);
                _timeSignatureFields.Add(newSigField);
            }
        }

        public override void Clear()
        {
            foreach (var graphic in _bpmFields)
            {
                Destroy(graphic.gameObject);
            }
            _bpmFields.Clear();
            _bpmFieldPool.Clear();
            _bpmChangeToField.Clear();
            foreach (var graphic in _timeSignatureFields)
            {
                Destroy(graphic.gameObject);
            }
            _timeSignatureFields.Clear();
            _timeSignatureFieldPool.Clear();
            _timeSigChangeToField.Clear();
        }

        private List<BpmField> _bpmFields;
        private List<TimeSignatureField> _timeSignatureFields;

        private Queue<BpmField> _bpmFieldPool;
        private Queue<TimeSignatureField> _timeSignatureFieldPool;

        private Dictionary<BpmChange, BpmField> _bpmChangeToField;
        private Dictionary<TimeSignatureChange, TimeSignatureField> _timeSigChangeToField;

        [Header("Graphics Options")] 
        [SerializeField] private float _verticalOffset;
        [SerializeField] private float _fontSize;
        [SerializeField] private BpmField _bpmFieldPrefab;
        [SerializeField] private TimeSignatureField _timeSignatureFieldPrefab;

        [Header("Advanced")] 
        [SerializeField] [Range(50, 500)] private int _initialisedFields;
        [SerializeField] private RectTransform _graphicsRoot;
 
        private void Awake()
        {
            _bpmFields = new List<BpmField>();
            _timeSignatureFields = new List<TimeSignatureField>();
            _bpmFieldPool = new Queue<BpmField>(_bpmFields);
            _timeSignatureFieldPool = new Queue<TimeSignatureField>(_timeSignatureFields);
            _bpmChangeToField = new Dictionary<BpmChange, BpmField>();
            _timeSigChangeToField = new Dictionary<TimeSignatureChange, TimeSignatureField>();
        }

        public override void Draw(EditorEngine data, Rect panel, float leftTime, float rightTime, int fromIndex = 0)
        {
            List<BpmChange> bpmChanges = data.BpmChanges;
            List<TimeSignatureChange> timeSigChanges = data.TimeSigChanges;

            // Kill objects whose corresponding events have been deleted
            foreach (var bpmField in _bpmFields)
            {
                if (bpmField.ChangeFlag == null || !data.BpmChanges.Contains(bpmField.ChangeFlag))
                {
                    if(bpmField.ChangeFlag != null && _bpmChangeToField.ContainsKey(bpmField.ChangeFlag))
                        _bpmChangeToField.Remove(bpmField.ChangeFlag);
                    bpmField.ChangeFlag = null;
                    bpmField.gameObject.SetActive(false);
                    _bpmFieldPool.Enqueue(bpmField);
                }
            }
            foreach (var timeSigField in _timeSignatureFields)
            {
                if (timeSigField.ChangeFlag == null || !data.TimeSigChanges.Contains(timeSigField.ChangeFlag))
                {
                    if(timeSigField.ChangeFlag != null && _timeSigChangeToField.ContainsKey(timeSigField.ChangeFlag))
                        _timeSigChangeToField.Remove(timeSigField.ChangeFlag);
                    timeSigField.ChangeFlag = null;
                    timeSigField.gameObject.SetActive(false);
                    _timeSignatureFieldPool.Enqueue(timeSigField);
                }
            }

            HashSet<BpmChange> _drawnBpmChanges = _bpmFields.Select(x => x.ChangeFlag).ToHashSet();
            HashSet<TimeSignatureChange> _drawnTimeSigChanges = _timeSignatureFields.Select(x => x.ChangeFlag).ToHashSet();

            // Draw at appropriate locations
            foreach (var bpmChange in bpmChanges)
            {
                if (bpmChange.Time < leftTime - DRAW_PADDING_SECONDS ||
                    bpmChange.Time > rightTime + DRAW_PADDING_SECONDS ||
                    bpmChange.Hide)
                {
                    if(_bpmChangeToField.ContainsKey(bpmChange))
                        _bpmChangeToField[bpmChange].gameObject.SetActive(false);
                    continue;
                }

                BpmField field;
                if (!_drawnBpmChanges.Contains(bpmChange))
                {
                    field = _bpmFieldPool.Dequeue();
                    _bpmChangeToField[bpmChange] = field;
                    field.ChangeFlag = bpmChange;
                    field.Lock = bpmChange.Lock;
                    field.SetText(bpmChange.Bpm.ToString());
                }
                else
                {
                    field = _bpmChangeToField[bpmChange];
                }

                float tx = MathUtility.InverseLerpUnclamped(leftTime, rightTime, bpmChange.Time);
                float x = Mathf.LerpUnclamped(panel.xMin, panel.xMax, tx);
                float y = panel.yMin - _verticalOffset;
                field.rectTransform.anchoredPosition = new Vector2(x, y);
                
                field.gameObject.SetActive(true);
            }

            foreach (var timeSigChange in timeSigChanges)
            {
                if (timeSigChange.Time < leftTime - DRAW_PADDING_SECONDS ||
                    timeSigChange.Time > rightTime + DRAW_PADDING_SECONDS ||
                    timeSigChange.Hide)
                {
                    if(_timeSigChangeToField.ContainsKey(timeSigChange))
                        _timeSigChangeToField[timeSigChange].gameObject.SetActive(false);
                    continue;
                }

                TimeSignatureField field;
                if (!_drawnTimeSigChanges.Contains(timeSigChange))
                {
                    field = _timeSignatureFieldPool.Dequeue();
                    _timeSigChangeToField[timeSigChange] = field;
                    field.ChangeFlag = timeSigChange;
                    var newSig = timeSigChange.TimeSignature;
                    field.SetText(newSig.Numerator.ToString(), newSig.Denominator.ToString());
                }
                else
                {
                    field = _timeSigChangeToField[timeSigChange];
                }

                float tx = MathUtility.InverseLerpUnclamped(leftTime, rightTime, timeSigChange.Time);
                float x = Mathf.LerpUnclamped(panel.xMin, panel.xMax, tx);
                float y = panel.yMin - _verticalOffset;
                field.rectTransform.anchoredPosition = new Vector2(x, y);
                
                field.gameObject.SetActive(true);
            }

        }
    }
}