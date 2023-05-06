using System.Threading.Tasks;
using RhythmEngine;
using Serialization;
using SimpleFileBrowser;
using Unity.VisualScripting;
using UnityEngine;

namespace IO
{
    public static class LevelDataIOManager
    {
        public static async Task<string> SaveLevelData(LevelData data)
        {
            bool cancel = false;
            string[] selectedPaths = null;
            FileBrowser.ShowSaveDialog(
                (paths => selectedPaths = paths),
                (() => cancel = true),
                FileBrowser.PickMode.Files
            );
            while (!cancel && selectedPaths == null)
            {
                await Task.Delay(5);
            }
            
            if (cancel || selectedPaths == null || selectedPaths.Length < 1) return ""; // redundant but IDE appeasement
            string path = selectedPaths[0];
            Task writeTask = WriteToJSONFile(data, path);
            await writeTask;
            if (!writeTask.IsCompletedSuccessfully)
            {
                Debug.LogError("Level data save failed");
                return "";
            }
            return path;
        }

        public static async Task<(LevelData, AudioClip)> LoadLevelData(string path)
        {
            LevelData levelData = await ReadFromJSONFile(path);
            string audioClipPath = levelData.SongData.ClipPath;
            AudioClip audioClip = await SongSelect.LoadAudioClip(audioClipPath);
            if (audioClip == null)
            {
                Debug.LogError($"AudioClip at {audioClipPath} connected to this level data cannot be found. Did you move it?");
                return (null, null);
            }

            return (levelData, audioClip);
        }

        private static async Task WriteToJSONFile(LevelData data, string path)
        {
            Debug.Log("Writing level data to file");
            Task saveTask = SerializationManager.Save(path, data);
            await saveTask;
            if(saveTask.IsCompletedSuccessfully)
                Debug.Log($"Successfully saved level data at {path}");
        }

        private static async Task<LevelData> ReadFromJSONFile(string path)
        {
            Debug.Log($"Reading file at path {path}");
            Task<LevelData> dataTask = SerializationManager.Load<LevelData>(path);
            LevelData data = await dataTask;
            if (!dataTask.IsCompletedSuccessfully)
            {
                Debug.LogError($"Could not parse file at {path}");
                return null;
            }
            return data;
        }
    }
}