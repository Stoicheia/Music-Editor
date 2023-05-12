using System;
using System.Collections.Generic;
using DefaultNamespace.Input;
using UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace RhythmEngine
{
    public class TimelineRecorder : MonoBehaviour
    {
        public event Action<bool> OnToggleRecord;
        
        [SerializeField] private TimelineUI _timeline;
        [SerializeField] private EditorEngine _engine;
        [SerializeField] private SongSeekerUI _songSeeker;
        [SerializeField] private ToolbarUI _toolbar;
        [SerializeField] private float _stacking = 0.05f;
        [SerializeField] private RectTransform _recordInstructions;

        private bool _isRecording;
        private Dictionary<int, EventNodeUI> _keyToActive;
        private Dictionary<float, int> _stackingSet;

        private void Awake()
        {
            _keyToActive = new Dictionary<int, EventNodeUI>();
            _stackingSet = new Dictionary<float, int>();
        }

        private void Start()
        {
            Keybinds.OnRecordPressed += HandlePressRecord;
            Keybinds.OnRecordKey += HandlePressKey;
            Keybinds.OnHoldKey += HandleHoldKey;
            Keybinds.OnReleaseKey += HandleReleaseKey;
            _recordInstructions.gameObject.SetActive(false);
        }

        private void HandlePressRecord()
        {
            if (_toolbar.ActiveOption != ToolbarOption.Record)
            {
                _toolbar.ActiveOption = ToolbarOption.Record;
            }
            _isRecording = !_isRecording;
            _recordInstructions.gameObject.SetActive(_isRecording);
            OnToggleRecord?.Invoke(_isRecording);
            if (_isRecording)
            {
                _songSeeker.UnpauseSong();
            }
            if (!_isRecording)
            {
                _toolbar.ActiveOption = ToolbarOption.Select;
                _songSeeker.PauseSong();
            }
            foreach (var key in _keyToActive.Keys)
            {
                _keyToActive[key] = null;
            }
            _stackingSet.Clear();
            _toolbar.ToggleSeeker(true);
        }
        
        private void HandleReleaseKey(int k)
        {
            if (!_isRecording) return;
            if (!_keyToActive.ContainsKey(k)) return;
            _keyToActive[k] = null;
        }

        private void HandleHoldKey(int k)
        {
            if (!_isRecording) return;
            if (!_keyToActive.ContainsKey(k)) return;
            foreach (var keyPair in _keyToActive)
            {
                if (keyPair.Value == null || keyPair.Key == 1) return;
                _timeline.ExtendEventNode(keyPair.Value, _songSeeker.SongTimeSeconds);
            }
        }

        private void HandlePressKey(int k)
        {
            if (!_isRecording) return;
            EventNodeUI eNode = _timeline.PlaceNew(_songSeeker.SongTimeSeconds, 0.75f, true);
            _keyToActive[k] = eNode;
            if(!_stackingSet.ContainsKey(eNode.Time)) _stackingSet.Add(eNode.Time, 0);
            eNode.Vertical -= _stackingSet[eNode.Time] * _stacking;
            eNode.Event.Vertical = eNode.Vertical;
            _stackingSet[eNode.Time]++;
        }
    }
}