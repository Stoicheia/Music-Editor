using System;
using System.Threading.Tasks;
using DefaultNamespace.LevelEditor;
using RhythmEngine;
using UnityEngine;
using UnityEngine.UI;

namespace IO
{
    public class LevelDataSelect : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private SongLoader _loader;
        [SerializeField] private EditorEngine _engine;
        [Header("Graphics")] 
        [SerializeField] private RectTransform _loadingMessage;
        [SerializeField] private RectTransform _loadSuccessMessage;
        [SerializeField] private RectTransform _loadFailureMessage;
        [SerializeField] private RectTransform _savingMessage;
        [SerializeField] private RectTransform _saveSuccessMessage;
        [SerializeField] private RectTransform _saveFailureMessage;

        [Header("Graphics Options")] 
        [SerializeField] [Range(100, 3000)] private int _messageFlashDuration = 999; 

        private void Start()
        {
            _loadingMessage.gameObject.SetActive(false);
            _loadSuccessMessage.gameObject.SetActive(false);
            _loadFailureMessage.gameObject.SetActive(false);
            _savingMessage.gameObject.SetActive(false);
            _saveSuccessMessage.gameObject.SetActive(false);
            _saveFailureMessage.gameObject.SetActive(false);
        }

        public async Task TrySaveLevelData()
        {
            var task = LevelDataIOManager.SaveLevelData(_engine.LevelData);
            await task;
            if (task.Result == "") return;
            RectTransform message = task.IsCompletedSuccessfully ? _saveSuccessMessage : _saveFailureMessage;
            
            message.gameObject.SetActive(true);
            await Task.Delay(_messageFlashDuration);
            message.gameObject.SetActive(false);

        }
        public async Task<(LevelData, AudioClip)> TryLoadLevelData()
        {
            var task = LevelDataIOManager.LoadLevelData();
            var result = await task;
            if(result != (null, null))
                _loader.ProcessExistingLevelData(result.Item1, result.Item2);
            if (task.Result == (null, null)) return default;
            RectTransform message = task.IsCompletedSuccessfully ? _loadSuccessMessage : _loadFailureMessage;

            message.gameObject.SetActive(true);
            await Task.Delay(_messageFlashDuration);
            message.gameObject.SetActive(false);
            
            return result;
        }

        public async void TryLoadLevelDataFromButton()
        {
            await TryLoadLevelData();
        }
        
        public async void TrySaveLevelDataFromButton()
        {
            await TrySaveLevelData();
        }
    }
}