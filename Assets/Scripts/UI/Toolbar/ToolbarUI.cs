using System;
using System.Collections.Generic;
using Rhythm;
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
        public event Action OnRequestBpmFlag;
        public event Action OnRequestTimeSignatureFlag;
        
        public ToolbarOption ActiveOption => _activeOption;
        public int Subdivision => _subdivision;
        
        [Header("Buttons")]
        [SerializeField] private List<ToolbarButtonUI> _buttons;
        [SerializeField] private Button _bpmFlagButton;
        [SerializeField] private Button _timeSigFlagButton;
        
        [Header("Options")]
        [SerializeField] private ToolbarOption _defaultOption;
        [SerializeField] private int _subdivision = 1;
        
        private ToolbarOption _activeOption;

        private void Start()
        {
            foreach (var b in _buttons)
            {
                b.OnSelect += ChangeOption;
            }

            _bpmFlagButton.onClick.AddListener(RequestBpmFlag);
            _timeSigFlagButton.onClick.AddListener(RequestTimeSignatureFlag);
            
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

        public void RequestBpmFlag()
        {
            OnRequestBpmFlag?.Invoke();
        }

        public void RequestTimeSignatureFlag()
        {
            OnRequestTimeSignatureFlag?.Invoke();
        }
    }
}