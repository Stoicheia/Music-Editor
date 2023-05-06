using System;
using UnityEngine;

namespace Rhythm
{
    [Serializable]
    public class SongAssetData
    {
        [SerializeField] public string ClipPath;
        [SerializeField] public string Name;
        [SerializeField] public string Author;
        [SerializeField] public float DefaultBpm;
        [SerializeField] public TimeSignature DefaultTimeSignature;
        
        public SongAssetData(string clipPath, string name, string author, float defaultBpm, TimeSignature defaultTimeSignature)
        {
            ClipPath = clipPath;
            Name = name;
            Author = author;
            DefaultBpm = defaultBpm;
            DefaultTimeSignature = defaultTimeSignature;
        }
    }
}