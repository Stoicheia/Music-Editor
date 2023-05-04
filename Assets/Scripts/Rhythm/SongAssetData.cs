using System;
using UnityEngine;

namespace Rhythm
{
    [Serializable]
    public class SongAssetData
    {
        [field: SerializeField] public string ClipPath { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public string Author { get; set; }
        [field: SerializeField] public float DefaultBpm { get; set; }
        [field: SerializeField] public TimeSignature DefaultTimeSignature { get; set; }
    }
}