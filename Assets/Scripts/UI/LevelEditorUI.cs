using System;
using System.Collections.Generic;
using System.Globalization;
using DefaultNamespace.Input;
using DefaultNamespace.LevelEditor;
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

        [Header("Systems")]
        [SerializeField] private SongAsset _loadedSong;
        [SerializeField] private EditorEngine _engine;
        [SerializeField] private SongSeekerUI _seeker;
        [SerializeField] private TimelineUI _timeline;
        [SerializeField] private SelectorUI _selector;
        [SerializeField] private ToolbarUI _toolbar;
        [SerializeField] private TimelineCommandManager _commandManager;
        [Header("Loader")] 
        [SerializeField] private SongLoader _loader;

        [Header("Graphics")] 
        [SerializeField] private RectTransform _noSongSelectedGraphic;
            
        private (LevelData, AudioClip) _loadData;

        private float _defaultBpm;

        private void Start()
        {
            //Init();
            _loader.OnLoadSong += (data, clip) =>
            {
                _loadData = (data, clip);
                Init(_loadData);
                _noSongSelectedGraphic.gameObject.SetActive(false);
            };
            Keybinds.OnUndoPressed += () =>
            {
                _commandManager.UndoCommand(_timeline, _engine);
            };
            Keybinds.OnRedoPressed += () =>
            {
                _commandManager.RedoCommand(_timeline, _engine);
            };
        }

        public void InitFromAsset(SongAsset asset = null)
        {
            var loadAsset = asset ? asset : _loadedSong;
            Init((new LevelData(loadAsset), loadAsset.Clip));
        }

        public void Init((LevelData, AudioClip) data)
        {
            _timeline.Clear();
            _loadData = data;
            var levelData = data.Item1;
            var clip = data.Item2;
            
            _seeker.InitWithAudioSource(_engine);
            _seeker.SetSong(clip);
            _engine.Load(levelData, clip);
            
            CurrentBpm = levelData.SongData.DefaultBpm;
            CurrentTimeSignature = levelData.SongData.DefaultTimeSignature;
            _defaultBpm = levelData.SongData.DefaultBpm;
            
            _timeline.Selector = _selector;
            _timeline.LevelEditor = this;
            _timeline.Engine = _engine;
            _timeline.SongSeeker = _seeker;
            _timeline.Toolbar = _toolbar;
            _timeline.Init();
        }
    }
}