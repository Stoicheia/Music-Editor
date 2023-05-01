using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rhythm;
using UnityEngine;
using Utility;

namespace RhythmEngine
{
    [RequireComponent(typeof(AudioSource))]
    public class EditorEngine : MonoBehaviour
    {
        public SongAsset Song => _song;
        public AudioSource AudioSource => _source;
        public List<RhythmEvent> Events => _events;
        

        [SerializeField] private SongAsset _song;
        private AudioSource _source;
        private List<RhythmEvent> _events;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _events = new List<RhythmEvent>();
        }

        public void SetSong(SongAsset song)
        {
            _song = song;
        }

        public void AddEvent(RhythmEvent e)
        {
            Events.Add(e);
            UpdateEvents();
        }

        public void UpdateEvents()
        {
            Events.Sort((x,y) => x.TimeSeconds.CompareTo(y.TimeSeconds)); // micro-inefficiency
        }
        
        public void RemoveEvent(RhythmEvent e)
        {
            Events.Remove(e);
            UpdateEvents();
        }

        public string SongTimeString()
        {
            return $"{StringUtility.SecondsPrettyString(AudioSource.time)}/{StringUtility.SecondsPrettyString(AudioSource.clip.length)}";
        }
    }
}