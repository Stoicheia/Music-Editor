using System;
using System.Collections.Generic;
using RhythmEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Utility;

namespace UI
{
    public class BarNumberDrawerUI : TimelineDrawerHelperUI
    {
        private bool _forceDrawThisFrame { get; set; }
        
        [SerializeField] [Range(10, 200)] private int _initialGraphicsCount;
        [SerializeField] private float _barNumberOffset = 100;
        [SerializeField] private float _barNumberSize = 15;
        [SerializeField] private RectTransform _barNumberGraphicsRoot;
        [SerializeField] private TextMeshProUGUI _barNumberTextPrefab;
        [SerializeField] private bool _draw;
        private List<RectTransform> _barNumberObjects;
        private List<TextMeshProUGUI> _barNumberTextObjects;
        private List<(float, int, int)> _subdivisions;

        private (float, float) _oldTimes;

        public override void Init(TimelineUI timeline, EditorEngine engine)
        {
            base.Init(timeline, engine);
            _oldTimes = (0, 0);
            _barNumberObjects = new List<RectTransform>();
            _barNumberTextObjects = new List<TextMeshProUGUI>();

            for (int i = 0; i < _initialGraphicsCount; i++)
            {
                RectTransform barNumberInstance = Instantiate(_barNumberTextPrefab, _barNumberGraphicsRoot)
                    .GetComponent<RectTransform>();
                _barNumberObjects.Add(barNumberInstance);
                _barNumberTextObjects.Add(barNumberInstance.GetComponent<TextMeshProUGUI>());
            }

            _engine.OnForceUpdate += () =>
            {
                _forceDrawThisFrame = true;
            };
        }

        public override void Clear()
        {
            foreach (var graphic in _barNumberObjects)
            {
                Destroy(graphic.gameObject);
            }
            _barNumberObjects.Clear();
            _barNumberTextObjects.Clear();
        }

        public void UpdateSubdivisions(List<(float, int, int)> subdivs)
        {
            _subdivisions = subdivs;
        }

        public override void Draw(EditorEngine data, Rect panel, float leftTime, float rightTime, int fromIndex)
        {
            if ((!_draw || (leftTime, rightTime) == _oldTimes) && !_forceDrawThisFrame) return;
            var subdivIndex = Math.Max(0, fromIndex - 1);
            _oldTimes = (leftTime, rightTime);
            int barTextIndex = 0;

            Vector2 panelLeft = new Vector2(panel.xMin, panel.y);
            Vector2 panelRight = new Vector2(panel.xMax, panel.y);
            
            // Disable all of the bar numbers
            for (int i = 0; i < _initialGraphicsCount; i++)
            {
                _barNumberObjects[i].gameObject.SetActive(false);
            }
            
            // Draw bar numbers within range
            while (subdivIndex < _subdivisions.Count && _subdivisions[subdivIndex].Item1 < rightTime)
            {
                float t = MathUtility.InverseLerpUnclamped(leftTime, rightTime, _subdivisions[subdivIndex].Item1);
                Vector2 timelinePos = Vector2.LerpUnclamped(panelLeft, panelRight, t) + Vector2.up * panel.size.y/2;
                RectTransform barTextObj = _barNumberObjects[barTextIndex];
                TextMeshProUGUI barText = _barNumberTextObjects[barTextIndex++];
                if (_subdivisions[subdivIndex].Item2 > 0)
                {
                    barTextObj.anchoredPosition = timelinePos + Vector2.up * _barNumberOffset;
                    barTextObj.gameObject.SetActive(true);
                    barText.fontSize = _barNumberSize;
                    barText.text = _subdivisions[subdivIndex].Item3.ToString();
                }
                else
                {
                    barTextObj.gameObject.SetActive(false);
                }
                
                subdivIndex++;
            }
        }
        
    }
}