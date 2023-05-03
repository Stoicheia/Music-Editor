using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public enum NodeState
    {
        Normal, Hovered, Selected
    }
    [RequireComponent(typeof(Image))]
    public abstract class EventNodeBase : MonoBehaviour, ISelectorInteractor, IPointerEnterHandler, IPointerExitHandler
    {
        protected static EventNodeBase Selected;
        public EventNodeUI Parent { get; set; }
        public RectTransform rectTransform => _rectTransform;
        protected NodeState State;
        protected Image Img;

        private RectTransform _rectTransform;

        [Header("Graphics Settings")] 
        [SerializeField] protected Sprite NormalSprite;
        [SerializeField] protected Sprite HoveredSprite;
        [SerializeField] protected Sprite SelectedSprite;

        protected virtual void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            Img = GetComponent<Image>();
            
            State = NodeState.Normal;
        }

        protected virtual void Update()
        {
            if (Parent.ParentUI.Toolbar.ActiveOption == ToolbarOption.Draw)
            {
                Img.sprite = NormalSprite;
                return;
            }
            Img.sprite = State switch
            {
                NodeState.Normal => NormalSprite,
                NodeState.Hovered => HoveredSprite,
                NodeState.Selected => SelectedSprite,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if(State != NodeState.Selected && Selected == null)
                State = NodeState.Hovered;
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if(State != NodeState.Selected)
                State = NodeState.Normal;
        }

        public virtual void Select(SelectInfo info, Vector2 pos, bool special = false)
        {
            
        }

        public virtual void Click(SelectInfo info, Vector2 pos)
        {
            State = NodeState.Selected;
            Selected = this;
        }

        public virtual void Move(SelectInfo info, Vector2 pos)
        {
            
        }

        public virtual void Place(SelectInfo info, Vector2 pos)
        {
            State = NodeState.Normal;
            Selected = null;
        }

        public virtual void RightClicked(SelectInfo info, Vector2 pos)
        {
            
        }
    }
}