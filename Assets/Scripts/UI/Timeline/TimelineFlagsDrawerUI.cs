using System;
using System.Collections.Generic;
using RhythmEngine;
using UnityEngine;
using Utility;

namespace UI
{
    public class TimelineFlagsDrawerUI : MonoBehaviour
    {
        private const float DRAW_PADDING_SECONDS = 1;
        public TimelineUI Timeline { get; set; }

        private List<BpmField> _bpmFields;
        private List<TimeSignatureField> _timeSignatureFields;

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

            for (int i = 0; i < _initialisedFields; i++)
            {
                BpmField newBpmField = Instantiate(_bpmFieldPrefab, _graphicsRoot);
                TimeSignatureField newSigField = Instantiate(_timeSignatureFieldPrefab, _graphicsRoot);
                
                _bpmFields.Add(newBpmField);
                _timeSignatureFields.Add(newSigField);
            }
        }

        public void Draw(EditorEngine data, Rect panel, float leftTime, float rightTime)
        {
            List<BpmChange> bpmChanges = data.BpmChanges;
            List<TimeSignatureChange> timeSigChanges = data.TimeSigChanges;
            
            // Reset state of all fields
            foreach (var bpmf in _bpmFields)
            {
                bpmf.ChangeFlag = null;
                bpmf.gameObject.SetActive(false);
            }
            foreach (var sigf in _timeSignatureFields)
            {
                sigf.ChangeFlag = null;
                sigf.gameObject.SetActive(false);
            }

            int bpmFieldIndex = 0;
            // Draw at appropriate locations
            foreach (var bpmChange in bpmChanges)
            {
                if (bpmChange.Time < leftTime - DRAW_PADDING_SECONDS ||
                    bpmChange.Time > rightTime + DRAW_PADDING_SECONDS)
                {
                    continue;
                }

                var field = _bpmFields[bpmFieldIndex++];
                field.ChangeFlag = bpmChange;

                float tx = MathUtility.InverseLerpUnclamped(leftTime, rightTime, bpmChange.Time);
                float x = Mathf.LerpUnclamped(panel.xMin, panel.xMax, tx);
                float y = panel.yMin - _verticalOffset;
                field.rectTransform.anchoredPosition = new Vector2(x, y);
                
                field.gameObject.SetActive(true);
            }

            int timeSigFieldIndex = 0;
            foreach (var timeSigChange in timeSigChanges)
            {
                if (timeSigChange.Time < leftTime - DRAW_PADDING_SECONDS ||
                    timeSigChange.Time > rightTime + DRAW_PADDING_SECONDS)
                {
                    continue;
                }

                var field = _timeSignatureFields[timeSigFieldIndex++];
                field.ChangeFlag = timeSigChange;

                float tx = MathUtility.InverseLerpUnclamped(leftTime, rightTime, timeSigChange.Time);
                float x = Mathf.LerpUnclamped(panel.xMin, panel.xMax, tx);
                float y = panel.yMin - _verticalOffset;
                field.rectTransform.anchoredPosition = new Vector2(x, y);
                
                field.gameObject.SetActive(true);
            }
        }
    }
}