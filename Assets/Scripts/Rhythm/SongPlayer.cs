using System;
using UnityEngine;

namespace Rhythm
{
    [RequireComponent(typeof(AudioSource))]
    public class SongPlayer : MonoBehaviour
    {
        private AudioSource _audio;
        private SongAsset _activeSong; //MOVE BASE SET TO LevelEditorUI.cs

        private void Awake()
        {
            _audio = GetComponent<AudioSource>();
        }

        public void SetSong(SongAsset song)
        {
            _activeSong = song;
            _audio.clip = song.Clip;
        }
    }
}