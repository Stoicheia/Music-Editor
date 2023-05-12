using System;
using System.Collections.Generic;
using DefaultNamespace.Input;
using Rhythm;
using RhythmEngine;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public enum ToolbarOption
    {
        Select, Draw, Record, Properties
    }

    public class ToolbarUI : MonoBehaviour
    {
        public event Action OnRequestBpmFlag;
        public event Action OnRequestTimeSignatureFlag;
        public event Action OnRequestOffsetFlag;
        public event Action<bool> OnToggleSeekerState;
        public event Action<(bool, int)> OnToggleSnapState;

        public ToolbarOption ActiveOption
        {
            get => _activeOption;
            set
            {
                ChangeOption(value);
            }
        }
        public int Subdivision => _subdivision;
        public bool SnapToGrid => _snapOn;

        [Header("Buttons")] [SerializeField] private List<ToolbarButtonUI> _buttons;
        [SerializeField] private Button _bpmFlagButton;
        [SerializeField] private Button _offsetFlagButton;
        [SerializeField] private Button _timeSigFlagButton;
        [SerializeField] private ToolbarToggle _seekerToggle;
        [SerializeField] private BeatSnapDivisorUI _snapDivisorSlider;
        [SerializeField] private Button _snapToggleButton;

        [Header("Options")] [SerializeField] private ToolbarOption _defaultOption;
        [SerializeField] private int _subdivision = 1;

        private ToolbarOption _activeOption;
        private bool _snapOn;

        private void Start()
        {
            foreach (var b in _buttons)
            {
                b.OnSelect += ChangeOption;
            }

            _bpmFlagButton.onClick.AddListener(RequestBpmFlag);
            _offsetFlagButton.onClick.AddListener(RequestOffsetFlag);
            _timeSigFlagButton.onClick.AddListener(RequestTimeSignatureFlag);
            _seekerToggle.OnToggle += HandleSeekerToggle;
            _snapDivisorSlider.OnChangeDivs += HandleBeatSnapChange;
            _snapToggleButton.onClick.AddListener(HandleToggleSnap);

            Keybinds.OnDrawPressed += TempDrawOn;
            Keybinds.OnDrawReleased += TempDrawOff;
            Keybinds.OnSeekLockToggle += ToggleSeeker;

            ChangeOption(_defaultOption);
        }

        private void TempDrawOn()
        {
            if(_activeOption == ToolbarOption.Select) ChangeOption(ToolbarOption.Draw);
        }

        private void TempDrawOff()
        {
            if (_activeOption == ToolbarOption.Draw) ChangeOption(ToolbarOption.Select);
        }

        private void HandleSeekerToggle(bool state)
        {
            OnToggleSeekerState?.Invoke(state);
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
        
        private void ToggleSeeker()
        {
            _seekerToggle.Toggle();
        }

        public void RequestBpmFlag()
        {
            OnRequestBpmFlag?.Invoke();
        }
        
        public void RequestOffsetFlag()
        {
            OnRequestOffsetFlag?.Invoke();
        }


        public void RequestTimeSignatureFlag()
        {
            OnRequestTimeSignatureFlag?.Invoke();
        }

        public void ToggleSeeker(bool s)
        {
            _seekerToggle.Toggle(s);
        }
        
        private void HandleBeatSnapChange(int value)
        {
            _subdivision = value;
            OnToggleSnapState?.Invoke((_snapOn, _subdivision));
        }

        private void HandleToggleSnap()
        {
            _snapOn = !_snapOn;
            OnToggleSnapState?.Invoke((_snapOn, _subdivision));
        }
        
    }
}