using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace SFX
{
    [RequireComponent(typeof(AudioSource))]
    public class TimelineClickSFX : MonoBehaviour
    {
        enum ClickFrequency
        {
            Bar = 1, Beat = 0, Subdivision = -1
        }
        
        [Header("Dependencies")]
        [SerializeField] private TimelineUI _timeline;
        [SerializeField] private ToolbarUI _toolbar;
        [Header("Settings")]
        [SerializeField] private ClickFrequency _clickFrequency;
        [SerializeField] private List<AudioClip> _clickSfx;

        private AudioSource _audio;

        private void Awake()
        {
            _audio = GetComponent<AudioSource>();
        }

        private void Start()
        {
            _timeline.OnPassDiv += HandlePassDiv;
        }

        private void HandlePassDiv(int val)
        {
            if (!_toolbar.ClickSFXOn) return;
            if (val >= (int)_clickFrequency)
            {
                _audio.PlayOneShot(_clickSfx[val + 1]);
            }
        }
    }
}