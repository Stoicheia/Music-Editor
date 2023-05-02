using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public enum ToolbarOption
    {
        Select, Draw
    }
    public class ToolbarUI : MonoBehaviour
    {
        public ToolbarOption ActiveOption => _activeOption;
        public int Subdivision => _subdivision;
        
        [SerializeField] private List<ToolbarButtonUI> _buttons;
        [SerializeField] private ToolbarOption _defaultOption;
        [SerializeField] private int _subdivision = 1;
        
        private ToolbarOption _activeOption;

        private void Start()
        {
            foreach (var b in _buttons)
            {
                b.OnSelect += ChangeOption;
            }
            ChangeOption(_defaultOption);
        }

        private void ChangeOption(ToolbarOption option)
        {
            _activeOption = option;
            foreach (var b in _buttons)
            {
                if(b.Option == option) b.SetGraphicActive();
                else b.SetGraphicInactive();
            }
        }
    }
}