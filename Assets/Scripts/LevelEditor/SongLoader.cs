using System;
using System.IO;
using System.Threading.Tasks;
using Rhythm;
using RhythmEngine;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

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
        
        public static async Task<AudioClip> LoadAudioClip(string path)
        {
            // Copy file to persistent path just in case (unused atm)
            string destinationPath = Path.Combine(Application.persistentDataPath, FileBrowserHelpers.GetFilename(path));
            FileBrowserHelpers.CopyFile( path, destinationPath);
            
            // Load file from original path
            AudioClip clip = null;
            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
            {
                uwr.SendWebRequest();
 
                try
                {
                    while (!uwr.isDone) await Task.Delay(5);
 
                    if (uwr.isNetworkError || uwr.isHttpError) Debug.Log($"{uwr.error}");
                    else
                    {
                        clip = DownloadHandlerAudioClip.GetContent(uwr);
                    }
                }
                catch (Exception err)
                {
                    Debug.Log($"{err.Message}, {err.StackTrace}");
                }
            }
            
            return clip;
        }
        
    }
}