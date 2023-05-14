using System.Collections.Generic;
using RhythmEngine;
using UnityEngine;

namespace UI
{
    public class TimelineHorizontalLinesDrawerUI : TimelineDrawerHelperUI
    {
        [SerializeField] private ToolbarUI _toolbar;
        [SerializeField] private RectTransform _horizontalLinePrefab;
        [SerializeField] private int _initialGraphicsCount = 20;
        [SerializeField] private RectTransform _graphicsRoot;

        private int _laneCount;
        private List<RectTransform> _horizontalLines;

        public override void Init(TimelineUI timeline, EditorEngine data)
        {
            base.Init(timeline, data);
            _horizontalLines = new List<RectTransform>();
            for (int i = 0; i < _initialGraphicsCount; i++)
            {
                var line = Instantiate(_horizontalLinePrefab, _graphicsRoot);
                _horizontalLines.Add(line);
            }
        }

        public override void Draw(EditorEngine data, Rect panel, float leftTime, float rightTime, int fromIndex = 0)
        {
            _laneCount = _toolbar.HorizontalLaneCount;
            float y1 = panel.yMin;
            float y2 = panel.yMax;
            for (int i = 0; i < _horizontalLines.Count; i++)
            {
                _horizontalLines[i].gameObject.SetActive(false);
            }
            for (int i = 0; i <= _laneCount; i++)
            {
                var line = _horizontalLines[i];
                _horizontalLines[i].gameObject.SetActive(true);
                float y = Mathf.Lerp(y1, y2, (float) i / _laneCount);
                line.anchoredPosition = new Vector2(0, y);
            }
        }
        
        public override void Clear()
        {
            foreach (var line in _horizontalLines)
            {
                Destroy(line.gameObject);
            }
            _horizontalLines.Clear();
        }

        public float SnapVertical(float value)
        {
            if (!_toolbar.HorizontalSnapOn) return value;
            float laneSize = 1.0f / _laneCount;
            return Mathf.FloorToInt(value * _laneCount) / (float) _laneCount + laneSize / 2;
        }
    }
}