using System;
using RhythmEngine;
using UnityEngine;
using Utility;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class TimelineSeeker : MonoBehaviour, ISelectorInteractor
    {
        public SongSeekerUI Audio { get; set; }

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Draw(Rect panel, float leftTime, float rightTime, float center, float height, float off)
        {
            float t = MathUtility.InverseLerpUnclamped(leftTime, rightTime, center);
            _rectTransform.anchoredPosition = 
                Vector2.LerpUnclamped(new Vector2(panel.xMin, panel.y), new Vector2(panel.xMax, panel.y), t) + Vector2.up * off;
            _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, height);
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
            //throw new System.NotImplementedException();
        }

        public void Place(SelectInfo info, Vector2 pos)
        {
            //throw new System.NotImplementedException();
        }

        public void RightClicked(SelectInfo info, Vector2 pos)
        {
            //throw new System.NotImplementedException();
        }
    }
}