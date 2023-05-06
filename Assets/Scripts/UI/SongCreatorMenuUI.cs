using System;
using DefaultNamespace.LevelEditor;
using IO;
using Rhythm;
using RhythmEngine;
using TMPro;
using UnityEngine;

namespace UI
{
    public class SongCreatorMenuUI : MonoBehaviour
    {
        
        
        [Header("Fields")]
        [SerializeField] private TMP_InputField _nameField;
        [SerializeField] private TMP_InputField _authorField;
        [SerializeField] private TMP_InputField _bpmField;
        [SerializeField] private TMP_InputField _timeSigField;
        [SerializeField] private TextMeshProUGUI _pathField;

        [Header("Config")] 
        [SerializeField] private SongSelect _songSelect;
        [SerializeField] private SongLoader _songLoader;

        private AudioClip _clip;
        private void Start()
        {
            _songSelect.OnChooseAudio += HandleChooseAudioClip;
        }

        public void TryCreateNewLevelData()
        {
            if (_clip == null)
            {
                Debug.LogWarning($"Please select a song first!");
                return;
            }
            SongAssetData songData = new SongAssetData(_pathField.text, _nameField.text,
                _authorField.text, ReadBpmText(_bpmField.text),
                ReadTimeSigText(_timeSigField.text, new TimeSignature(4, 4)));
            _songLoader.ProcessNewSongData(songData, _clip);
            gameObject.SetActive(false);
        }

        private void HandleChooseAudioClip(AudioClip clip, string path)
        {
            _pathField.text = path;
            _clip = clip;
        }

        private float ReadBpmText(string bpm, float @default = 120)
        {
            if (float.TryParse(bpm, out float value)) return value;
            return @default;
        }

        private TimeSignature ReadTimeSigText(string tsig, TimeSignature @default)
        {
            int numerator;
            int denominator;
            string[] tsigComponents = tsig.Split("/");
            if (tsigComponents.Length != 2) return @default;
            if (!int.TryParse(tsigComponents[0], out numerator)) return @default;
            if (!int.TryParse(tsigComponents[1], out denominator)) return @default;
            return new TimeSignature(numerator, denominator);
        }
    }
}