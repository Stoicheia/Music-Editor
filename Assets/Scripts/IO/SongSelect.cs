using UnityEngine;
using SimpleFileBrowser;

namespace IO
{
    public class SongSelect : MonoBehaviour
    {
        public void OpenSong()
        {
            FileBrowser.SetFilters( true, new FileBrowser.Filter( "Images", ".mp3", ".wav" ));
            FileBrowser.ShowLoadDialog(OnLoadSong, () => Debug.Log("meh"), FileBrowser.PickMode.Files);
        }

        private void OnLoadSong(string[] paths)
        {
            Debug.Log(paths[0]);
        }
    }
}