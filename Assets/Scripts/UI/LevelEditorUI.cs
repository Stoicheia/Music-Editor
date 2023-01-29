using System;
using Rhythm;
using RhythmEngine;
using UnityEngine;

namespace UI
{
    public class LevelEditorUI : MonoBehaviour
    {
        [SerializeField] private SongAsset _loadedSong;
        [SerializeField] private EditorEngine _engine;
        [SerializeField] private SongSeekerUI _seeker;
        [SerializeField] private TimelineUI _timeline;
        [SerializeField] private SelectorUI _selector;

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
        }
    }
}