using System;
using UnityEngine;

namespace Rhythm
{
    [CreateAssetMenu(fileName = "Song", menuName = "Song")]
    public class SongAsset : ScriptableObject
    {
        public AudioClip Clip => _clip;
        public string Name => _name;
        public string Author => _author;
        public float DefaultBpm => _defaultBpm;
        public TimeSignature DefaultTimeSignature => _defaultTimeSignature;
        [SerializeField] private AudioClip _clip;
        [SerializeField] private string _name;
        [SerializeField] private string _author;
        [SerializeField][Range(30, 300)] private float _defaultBpm;
        [SerializeField] private TimeSignature _defaultTimeSignature;
    }
}