using System;
using System.Globalization;
using Rhythm;
using RhythmEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace UI
{
    public class LevelEditorUI : MonoBehaviour
    {
        public float CurrentBpm { get; set; }
        public TimeSignature CurrentTimeSignature { get; set; }
        
        [SerializeField] private SongAsset _loadedSong;
        [SerializeField] private EditorEngine _engine;
        [SerializeField] private SongSeekerUI _seeker;
        [SerializeField] private TimelineUI _timeline;
        [SerializeField] private SelectorUI _selector;
        [SerializeField] private ToolbarUI _toolbar;
        [SerializeField] private TMP_InputField _bpmField;

        private float _defaultBpm;

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            _seeker.InitWithAudioSource(_engine);
            _seeker.SetSong(_loadedSong);
            _engine.SetSong(_loadedSong);
            _timeline.Selector = _selector;
            CurrentBpm = _loadedSong.DefaultBpm;
            CurrentTimeSignature = _loadedSong.DefaultTimeSignature;
            _defaultBpm = _loadedSong.DefaultBpm;
            _timeline.LevelEditor = this;
            _timeline.Engine = _engine;
            _timeline.SongSeeker = _seeker;
            _timeline.Toolbar = _toolbar;
            
            if (_bpmField == null) CurrentBpm = 120;
            else
            {
                _bpmField.onValueChanged.AddListener(s =>
                {
                    try
                    {
                        float.TryParse(s, out float bpmValue);
                        CurrentBpm = bpmValue;
                    }
                    catch
                    {
                        Debug.LogError($"Invalid BPM Value. Changing to default.");
                        CurrentBpm = _defaultBpm;
                        _bpmField.text = _defaultBpm.ToString(CultureInfo.InvariantCulture);
                    }
                });
            }
            _timeline.Init();
        }
    }
}