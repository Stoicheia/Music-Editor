using System;
using System.Collections.Generic;
using Rhythm;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;

namespace UI
{
    
    
    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class EventNodeUI : EventNodeBase, ISelectorInteractor, IPointerEnterHandler, IPointerExitHandler
    {
        public static RectTransform ExtenderRoot;
        public event Action<EventNodeUI> OnClick;
        public event Action<EventNodeUI> OnSelect;
        public event Action<EventNodeUI, Vector2> OnMove;
        public event Action<EventNodeUI> OnRightClick;
        public event Action<EventNodeUI> OnRequestDelete;
        public event Action<EventNodeUI, Vector2> OnRequestExtension;
        public event Action<EventNodeUI> OnPlace;
        
        public RhythmEvent Event { get; set; }
        public float Vertical { get; set; }
        public float Time => Event.TimeSeconds;
        public RectTransform ReferenceTransform { get; set; }
        public TimelineUI ParentUI { get; set; }
        public float ExtenderHeight { get; set; }
        public float NodeRadius { get; set; }
        
        [Header("Graphics")]
        [SerializeField] private ExtenderNodeUI _extenderPrefab;
        [SerializeField] private EndNodeUI _endNodePrefab;

        private ExtenderNodeUI _extenderInstance;
        private EndNodeUI _endNodeInstance;

        protected override void Awake()
        {
            base.Awake();
            Img = GetComponent<Image>();
            State = NodeState.Normal;
        }

        public void Init()
        {
            Parent = this;

            _extenderInstance = Instantiate(_extenderPrefab, ExtenderRoot);
            _endNodeInstance = Instantiate(_endNodePrefab, transform.parent);
            _endNodeInstance.transform.SetAsFirstSibling();
            
            _extenderInstance.gameObject.SetActive(false);
            _endNodeInstance.gameObject.SetActive(false);

            _extenderInstance.Parent = this;
            _endNodeInstance.Parent = this;
            Children = new List<EventNodeBase>() {_extenderInstance, _endNodeInstance};

            _extenderInstance.rectTransform.pivot = new Vector2(0, 0.5f);
        }

        protected override void Update()
        {
            base.Update();
        }

        public void DestroyChildren()
        {
            Destroy(_extenderInstance.gameObject);
            Destroy(_endNodeInstance.gameObject);
        }

        public void Draw(Rect rect, float leftTime, float rightTime, float vertical)
        {
            float t = MathUtility.InverseLerpUnclamped(leftTime, rightTime, Event.TimeSeconds);
            float x = Mathf.LerpUnclamped(rect.xMin, rect.xMax, t);
            float y = Mathf.LerpUnclamped(rect.yMin, rect.yMax, vertical);
            
            if (!Event.HasDuration)
            {
                _extenderInstance.gameObject.SetActive(false);
                _endNodeInstance.gameObject.SetActive(false);
                
                rectTransform.anchoredPosition = new Vector2(x, y);
            }
            else
            {
                _extenderInstance.gameObject.SetActive(true);
                _endNodeInstance.gameObject.SetActive(true);

                float tEnd = MathUtility.InverseLerpUnclamped(leftTime, rightTime, Event.TimeSeconds + Event.DurationSeconds);
                float xEnd = Mathf.LerpUnclamped(rect.xMin, rect.xMax, tEnd);
                
                rectTransform.anchoredPosition = new Vector2(x, y);
                _endNodeInstance.rectTransform.anchoredPosition = new Vector2(xEnd, y);
                _endNodeInstance.rectTransform.sizeDelta = new Vector2(NodeRadius * 2, NodeRadius * 2);
                _extenderInstance.rectTransform.anchoredPosition = new Vector2(x, y);
                _extenderInstance.rectTransform.sizeDelta = new Vector2(xEnd - x, ExtenderHeight);
            }
        }

        public void DisableGraphics()
        {
            gameObject.SetActive(false);
            _extenderInstance.gameObject.SetActive(false);
            _endNodeInstance.gameObject.SetActive(false);
        }

        public override void Select(SelectInfo info, Vector2 pos, bool special)
        {
            OnSelect?.Invoke(this);
        }

        public override void Click(SelectInfo info, Vector2 pos)
        {
            base.Click(info, pos);
            OnClick?.Invoke(this);
        }

        public override void Move(SelectInfo info, Vector2 pos)
        {
            OnMove?.Invoke(this, pos);
        }

        public override void Place(SelectInfo info, Vector2 pos)
        {
            base.Place(info, pos);
            OnPlace?.Invoke(this);
        }

        public override void RightClicked(SelectInfo info, Vector2 pos)
        {
            OnRightClick?.Invoke(this);
        }
        
        public void ForceDelete(SelectInfo info, Vector2 pos)
        {
            OnRequestDelete?.Invoke(this);
        }

        public void RequestExtension(SelectInfo info, Vector2 pos)
        {
            OnRequestExtension?.Invoke(this, pos);
        }
    }
}