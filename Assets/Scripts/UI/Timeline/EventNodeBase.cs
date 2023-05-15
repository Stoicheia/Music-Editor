using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UserInput;
using Object = System.Object;

namespace UI
{
    public enum NodeState
    {
        Normal, Hovered, Selected
    }
    [RequireComponent(typeof(Image))]
    public abstract class EventNodeBase : MonoBehaviour, ISelectorInteractor, IPointerEnterHandler, IPointerExitHandler, ISelectionBoxInteractor
    {
        protected static EventNodeBase Selected;
        public EventNodeUI Parent { get; set; }
        public List<EventNodeBase> Children { get; set; }
        public RectTransform rectTransform => _rectTransform;
        protected NodeState State;
        protected Image Img;

        private RectTransform _rectTransform;

        private bool _eventIsLastSelected;

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

        private void OnEnable()
        {
            SelectorUI.OnSelectObject += HandleObjectSelected;
            RightClickDragDeleteManager.Interactors.Add(this);
        }

        private void OnDisable()
        {
            SelectorUI.OnSelectObject -= HandleObjectSelected;
            RightClickDragDeleteManager.Interactors.Remove(this);
        }

        protected virtual void Update()
        {
            if (Parent.ParentUI.Toolbar.ActiveOption == ToolbarOption.Draw)
            {
                Img.sprite = NormalSprite;
                return;
            }
            if (Parent.ParentUI.Toolbar.ActiveOption == ToolbarOption.Properties)
            {
                Img.sprite = State switch
                {
                    NodeState.Normal => NormalSprite,
                    NodeState.Hovered => HoveredSprite,
                    NodeState.Selected => SelectedSprite,
                    _ => throw new ArgumentOutOfRangeException()
                };
                if (_eventIsLastSelected) Img.sprite = SelectedSprite;
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
        
        private void HandleObjectSelected(ISelectorInteractor obj)
        {
            EventNodeBase node = obj as EventNodeBase;
            if (node == null) return;
            _eventIsLastSelected = node.Parent.Event == Parent.Event;
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if(State != NodeState.Selected && Selected == null && Parent.ParentUI.Toolbar.ActiveOption != ToolbarOption.Properties)
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

        public void MultiSelect()
        {
            Parent.ForceDelete(new SelectInfo(this), _rectTransform.anchoredPosition);
        }

        public Vector2 GetSelectionPoint()
        {
            return rectTransform.position;
        }
    }
}