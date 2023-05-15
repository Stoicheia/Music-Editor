using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ToolbarButtonUI : MonoBehaviour
    {
        public Action<ToolbarOption> OnSelect;
        public ToolbarOption Option => _option;
        
        [Header("Function")]
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;
        [SerializeField] private ToolbarOption _option;
        [Header("Graphics")] 
        [SerializeField] private Sprite _activeSprite;
        [SerializeField] private Sprite _inactiveSprite;
        [SerializeField] private RectTransform _activeGraphics;

        private void Start()
        {
            _button.onClick.AddListener(InvokeClick);
        }

        private void InvokeClick()
        {
            OnSelect?.Invoke(_option);
        }

        public void SetGraphicActive()
        {
            if (_activeSprite != null) _image.sprite = _activeSprite;
            else _image.color = Color.white;
            _activeGraphics.gameObject.SetActive(true);
        }

        public void SetGraphicInactive()
        {
            if (_inactiveSprite != null) _image.sprite = _inactiveSprite;
            else _image.color = Color.gray;
            _activeGraphics.gameObject.SetActive(false);
        }
    }
}