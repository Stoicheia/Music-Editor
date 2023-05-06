using System;
using System.IO;
using System.Threading.Tasks;
using DefaultNamespace.LevelEditor;
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
            Task<AudioClip> loadTask = SongLoader.LoadAudioClip(path);
            await loadTask;
            var clip = loadTask.Result;
            if (clip != null)
            {
                _selectedSong = clip;
                _songPath = path;
            }

            return clip;
        }

        
        
        
    }
}