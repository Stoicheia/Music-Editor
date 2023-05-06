using System.Threading.Tasks;
using DefaultNamespace.LevelEditor;
using RhythmEngine;
using UnityEngine;

namespace IO
{
    public class LevelDataSelect : MonoBehaviour
    {
        [SerializeField] private SongLoader _loader;
        [SerializeField] private EditorEngine _engine;

        public async Task TrySaveLevelData()
        {
            await LevelDataIOManager.SaveLevelData(_engine.LevelData);
        }
        public async Task<(LevelData, AudioClip)> TryLoadLevelData()
        {
            var result = await LevelDataIOManager.LoadLevelData();
            if(result != (null, null))
                _loader.ProcessExistingLevelData(result.Item1, result.Item2);
            return result;
        }

        public void TryLoadLevelDataFromButton()
        {
            TryLoadLevelData();
        }
        
        public void TrySaveLevelDataFromButton()
        {
            TrySaveLevelData();
        }
    }
}