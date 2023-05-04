using System;
using Rhythm;
using RhythmEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI
{
    [RequireComponent(typeof(Slider))]
    public class SongSeekerUI : MonoBehaviour
    {
        public float ScrollSpeedMultiplier { get; set; }
        
        private Slider _slider;
        private AudioSource _audio;

        [Header("Graphics")]
        [SerializeField] private TextMeshProUGUI _timeField;


        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }

        private void Update()
        {
            if (_audio == null)
            {
                _timeField.text = "";
                return;
            }
            
            _timeField.text = _audio.clip == null ? "0:00/0:00" : 
                $"{StringUtility.SecondsPrettyString(_audio.time)}/{StringUtility.SecondsPrettyString(_audio.clip.length)}";
            

            _slider.value = _audio.time / _audio.clip.length;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                TogglePause();
            }
        }

        public void InitWithAudioSource(EditorEngine source)
        {
            _audio = source.AudioSource;
            _slider.onValueChanged.AddListener(UpdateSongTime);
        }

        public void SetSong(SongAsset song)
        {
            _audio.clip = song.Clip;
            _audio.Play();
            _audio.Pause();
        }
        
        public void PauseSong()
        {
            _audio.Pause();
        }

        public void UnpauseSong()
        {
            _audio.UnPause();
        }

        public void TogglePause()
        {
            if(_audio.isPlaying) _audio.Pause();
            else _audio.UnPause();
        }

        public float SongTimeSeconds => _audio.time;

        public float SongLengthSeconds => _audio.clip.length;

        private void UpdateSongTime(float val)
        {
            var sliderVal = val / _slider.maxValue;
            sliderVal = Mathf.Clamp(sliderVal, 0, _audio.clip.length);
            var time = sliderVal * _audio.clip.length;
            if (time > _audio.clip.length)
            {
                PauseSong();
                return;
            }
            _audio.time = time;
        }

        public void Scroll(float s)
        {
            UpdateSongTime(_audio.time + s);
        }
    }
}