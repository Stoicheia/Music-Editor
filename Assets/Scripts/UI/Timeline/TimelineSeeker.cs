using System;
using RhythmEngine;
using UnityEngine;
using Utility;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class TimelineSeeker : TimelineDrawerHelperUI, ISelectorInteractor
    {
        public event Action<float> OnMove;
        public event Action OnRelease;
        
        public SongSeekerUI Audio { get; set; }
        public bool SuppressMouseMove { get; set; }

        private RectTransform _rectTransform;
        [field: SerializeField] private float Offset { get; set; }
        [field: SerializeField] private float Height { get; set; }

        private Rect _panel;
        private float _left;
        private float _right;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void SetConfig(float h, float o)
        {
            Height = h;
            Offset = o;
        }

        public override void Clear()
        {
            
        }

        public override void Draw(EditorEngine _, Rect panel, float leftTime, float rightTime, int fromIndex)
        {
            _panel = panel;
            _left = leftTime;
            _right = rightTime;
            
            float t = MathUtility.InverseLerpUnclamped(leftTime, rightTime, Audio.SongTimeSeconds);
            _rectTransform.anchoredPosition = 
                Vector2.LerpUnclamped(new Vector2(panel.xMin, panel.y), new Vector2(panel.xMax, panel.y), t) + Vector2.up * Offset;
            _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, Height);
        }
        
        public void Select(SelectInfo info, Vector2 pos, bool special = false)
        {
            //throw new System.NotImplementedException();
        }

        public void Click(SelectInfo info, Vector2 pos)
        {
            //throw new System.NotImplementedException();
        }

        public void Move(SelectInfo info, Vector2 pos)
        {
            MoveTo(pos);
        }

        public void MoveTo(Vector2 pos)
        {
            var (time, _) = MathUtility.PositionToTime(
                pos,
                Timeline.RelativeTransform,
                _panel,
                _left,
                _right,
                null,
                true,
                true
            );
            
            if(!SuppressMouseMove)
                Audio.SetTime(time);
            OnMove?.Invoke(time);
        }

        public void Place(SelectInfo info, Vector2 pos)
        {
            OnRelease?.Invoke();
        }

        public void RightClicked(SelectInfo info, Vector2 pos)
        {
            //throw new System.NotImplementedException();
        }
    }
}