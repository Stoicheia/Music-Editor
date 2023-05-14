using System;
using RhythmEngine;
using TMPro;
using UnityEngine;

namespace UI
{
    public class SongTitleUI : MonoBehaviour
    {
        [Header("Dependencies")] 
        [SerializeField] private EditorEngine _levelEditor;
        [Header("Graphics")] 
        [SerializeField] private TextMeshProUGUI _titleField;

        private void Start()
        {
            _levelEditor.OnLoad += HandleLoadSong;
        }

        private void HandleLoadSong(LevelData data)
        {
            string author = data.SongData.Author;
            string songName = data.SongData.Name;
            if (author == "") author = "Unknown author";
            if (songName == "") songName = "Untitled";
            string prettyString = $"{author} - {songName}";
            _titleField.text = prettyString;
        }
    }
}