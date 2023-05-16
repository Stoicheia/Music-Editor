using System;
using System.Collections.Generic;
using DefaultNamespace.Input;
using UI;
using UnityEngine;

namespace RhythmEngine
{
    public class TimelineRecorder : MonoBehaviour
    {
        public event Action<bool> OnToggleRecord;
        
        [SerializeField] private TimelineUI _timeline;
        [SerializeField] private TimelineHorizontalLinesDrawerUI _horizontalLines;
        [SerializeField] private EditorEngine _engine;
        [SerializeField] private SongSeekerUI _songSeeker;
        [SerializeField] private ToolbarUI _toolbar;
        //[SerializeField] private float _stacking = 0.05f;
        [SerializeField] private RectTransform _recordInstructions;

        private bool _isRecording;
        private Dictionary<Keybinds.RecordKey, EventNodeUI> _keyToActive;
        //private Dictionary<float, int> _stackingSet;

        private void Awake()
        {
            _keyToActive = new Dictionary<Keybinds.RecordKey, EventNodeUI>();
            //_stackingSet = new Dictionary<float, int>();
        }

        private void Start()
        {
            Keybinds.OnRecordPressed += HandlePressRecord;
            Keybinds.OnRecordKey += HandlePressKey;
            Keybinds.OnHoldKey += HandleHoldKey;
            Keybinds.OnReleaseKey += HandleReleaseKey;
            _recordInstructions.gameObject.SetActive(false);
        }

        private void Update()
        {
            if(_isRecording && !_songSeeker.IsPlaying) HandlePressRecord();
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
            _keyToActive.Clear();
            //_stackingSet.Clear();
            _toolbar.ToggleSeeker(true);
        }
        
        private void HandleReleaseKey(Keybinds.RecordKey keyInfo)
        {
            if (!_isRecording) return;
            if (!_keyToActive.ContainsKey(keyInfo)) return;
            _keyToActive[keyInfo] = null;
        }

        private void HandleHoldKey(Keybinds.RecordKey keyInfo)
        {
            if (!_isRecording) return;
            foreach (var keyPair in _keyToActive)
            {
                if (keyPair.Value == null || !keyPair.Key.Hold || keyPair.Key != keyInfo) continue;
                _timeline.ExtendEventNode(keyPair.Value, _songSeeker.SongTimeSeconds + _toolbar.Offset/1000);
            }
        }

        private void HandlePressKey(Keybinds.RecordKey keyInfo)
        {
            if (!_isRecording) return;
            if (keyInfo.Lane >= _horizontalLines.LaneCount) return;
            float vertical = _horizontalLines.LaneToVertical(keyInfo.Lane);
            EventNodeUI eNode = _timeline.PlaceNew(_songSeeker.SongTimeSeconds + _toolbar.Offset/1000, vertical, true);
            _keyToActive[keyInfo] = eNode;
            eNode.Event.Vertical = eNode.Vertical;
        }
    }
}