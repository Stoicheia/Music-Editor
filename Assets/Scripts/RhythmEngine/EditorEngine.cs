using System;
using System.Threading.Tasks;
using Rhythm;
using UnityEngine;

namespace RhythmEngine
{
    [RequireComponent(typeof(AudioSource))]
    public class EditorEngine : MonoBehaviour
    {
        public LevelData LevelData { get; set; }
        public SongAsset Song => _song;
        public AudioSource AudioSource => _source;
        [SerializeField] private SongAsset _song;
        private AudioSource _source;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        public void SetData(LevelData data)
        {
            LevelData = data;
        }

        public void SetSong(SongAsset song)
        {
            _song = song;
        }
    }
}