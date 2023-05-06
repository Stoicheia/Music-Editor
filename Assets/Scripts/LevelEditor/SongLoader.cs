using System;
using Rhythm;
using RhythmEngine;
using TMPro;
using UnityEngine;

namespace DefaultNamespace.LevelEditor
{
    public class SongLoader : MonoBehaviour
    {
        public event Action<LevelData, AudioClip> OnLoadSong;
        
        public void ProcessNewSongData(SongAssetData songAsset, AudioClip loadedClip)
        {
            LevelData newLevelData = new LevelData(songAsset);
            OnLoadSong?.Invoke(newLevelData, loadedClip);
        }

        public void ProcessExistingLevelData(LevelData data, AudioClip loadedClip)
        {
            OnLoadSong?.Invoke(data, loadedClip);
        }
        
    }
}