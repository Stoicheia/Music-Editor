using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ToolbarToggle : MonoBehaviour
    {
        public event Action<bool> OnToggle;

        [SerializeField] private bool _defaultState;
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;
        [SerializeField] private Sprite _onSprite;
        [SerializeField] private Sprite _offSprite;

        private bool _state;
        public bool State
        {
            get => _state;
            set
            {
                _state = value;
                _image.sprite = _state ? _onSprite : _offSprite;
            }
        }

        private void OnEnable()
        {
            State = _defaultState;
            _button.onClick.AddListener(Toggle);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(Toggle);
        }

        public void Toggle()
        {
            State = !State;
            OnToggle?.Invoke(State);
        }

        public void Toggle(bool b)
        {
            State = b;
            OnToggle?.Invoke(State);
        }
        
    }
}