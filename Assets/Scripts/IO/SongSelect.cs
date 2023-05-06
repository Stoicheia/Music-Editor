using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using SimpleFileBrowser;
using UnityEngine.Networking;

namespace IO
{
    public class SongSelect : MonoBehaviour
    {
        public event Action<AudioClip, string> OnChooseAudio;
        private AudioClip _selectedSong;
        private string _songPath;
        
        public void OpenSong()
        {
            FileBrowser.SetFilters( true, new FileBrowser.Filter( "WAV Files", ".wav" ));
            FileBrowser.ShowLoadDialog(OnLoadSong, () => Debug.Log("meh"), FileBrowser.PickMode.Files);
        }

        private void Update()
        {
            if (_selectedSong != null)
            {
                OnChooseAudio?.Invoke(_selectedSong, _songPath);
                _selectedSong = null;
            }
        }

        private void OnLoadSong(string[] paths)
        {
            GetClip(paths[0]); // async not compatible at all with FileBrowser library. All hack attempts except this one failed.
        }

        private async Task<AudioClip> GetClip(string path)
        {
            Task<AudioClip> loadTask = LoadAudioClip(path);
            await loadTask;
            var clip = loadTask.Result;
            if (clip != null)
            {
                _selectedSong = clip;
                _songPath = path;
            }

            return clip;
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