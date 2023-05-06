using System;
using UnityEngine;

namespace Rhythm
{
    [CreateAssetMenu(fileName = "Song", menuName = "Song")]
    public class SongAsset : ScriptableObject
    {
        public AudioClip Clip => _clip;
        public string Name => _data.Name;
        public string Author => _data.Author;
        public float DefaultBpm => _data.DefaultBpm;
        public TimeSignature DefaultTimeSignature => _data.DefaultTimeSignature;
        public SongAssetData Data => _data;

        [SerializeField] private AudioClip _clip;
        [SerializeField] private SongAssetData _data;
    }
}