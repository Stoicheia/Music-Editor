using UnityEngine;

namespace UI
{
    public class OptionsUI : MonoBehaviour
    {
        [SerializeField] private SongCreatorMenuUI _songCreator;
        
        public void OpenSongCreatorMenu()
        {
            _songCreator.gameObject.SetActive(true);
        }
    }
}