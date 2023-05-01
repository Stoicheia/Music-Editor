using System;
using Rhythm;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public interface ISelectable
    {
        
    }
    
    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class EventNodeUI : MonoBehaviour, ISelectorInteractor
    {
        public event Action<EventNodeUI> OnClick;
        public event Action<EventNodeUI> OnSelect;
        public event Action<EventNodeUI, Vector2> OnMove;
        public event Action<EventNodeUI> OnRightClick;
        
        public RhythmEvent Event { get; set; }
        public RectTransform rectTransform => _rectTransform;

        private Image _img;
        private RectTransform _rectTransform;

        [Header("Graphics")] 
        [SerializeField] private Color _defaultSprite;
        [SerializeField] private Color _selectedSprite;

        private void Awake()
        {
            _img = GetComponent<Image>();
            _rectTransform = GetComponent<RectTransform>();
            _img.color = _defaultSprite;
        }

        public void Select(SelectInfo info, Vector2 pos, bool special)
        {
            OnSelect?.Invoke(this);
        }

        public void Click(SelectInfo info, Vector2 pos)
        {
            OnClick?.Invoke(this);
            _img.color = _selectedSprite;
        }

        public void Move(SelectInfo info, Vector2 pos)
        {
            OnMove?.Invoke(this, pos);
        }

        public void Place(SelectInfo info, Vector2 pos)
        {
            _img.color = _defaultSprite;
        }

        public void RightClicked(SelectInfo info, Vector2 pos)
        {
            OnRightClick?.Invoke(this);
        }
    }
}