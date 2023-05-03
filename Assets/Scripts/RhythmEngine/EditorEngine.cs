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
        public List<RhythmEvent> Events => _levelData.Events;
        public List<BpmChange> BpmChanges => _levelData.GetBpmChanges();
        public List<TimeSignatureChange> TimeSigChanges => _levelData.GetTimeSigChanges();


        [SerializeField] private SongAsset _song;

        private LevelData _levelData;
        
        private AudioSource _source;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _levelData = new LevelData(_song.DefaultBpm, _song.DefaultTimeSignature);
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

        public void ClearLevelData()
        {
            _levelData.Clear();
        }

        public float GetBpm(float time)
        {
            // Binary search to find last time before input time
            int index = 0;
            int count = BpmChanges.Count;
            for (int k = count / 2; k > 0; k /= 2)
            {
                while (index + k < count && BpmChanges[index + k].Time < time)
                {
                    index += k;
                }
            }

            return BpmChanges[index].Bpm;
        }

        public TimeSignature GetTimeSignature(float time)
        {
            // Binary search to find last time before input time
            int index = 0;
            int count = TimeSigChanges.Count;
            for (int k = count / 2; k > 0; k /= 2)
            {
                while (index + k < count && TimeSigChanges[index + k].Time < time)
                {
                    index += k;
                }
            }

            return TimeSigChanges[index].TimeSignature;
        }
    }
}