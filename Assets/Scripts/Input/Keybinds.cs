using System;
using UnityEngine;

namespace DefaultNamespace.Input
{
    public class Keybinds : MonoBehaviour
    {
        public static event Action OnDrawPressed;
        public static event Action OnDrawReleased;
        public static event Action OnSeekLockToggle;
        public static event Action OnTogglePause;
        public static bool Shift;
        
        private static Keybinds _instance;
        public static Keybinds Instance
        {
            get
            {
                if (_instance == null) throw new Exception("Trying to access keybinds, but no keybinds object exists.");
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        [SerializeField] private KeyCode _draw = KeyCode.LeftAlt;
        [SerializeField] private KeyCode _seekLockToggle = KeyCode.S;
        [SerializeField] private KeyCode _togglePause = KeyCode.Space;
        [SerializeField] private KeyCode _shift = KeyCode.LeftShift;
        
        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Update()
        {
            if(UnityEngine.Input.GetKeyDown(_draw)) OnDrawPressed?.Invoke();
            if(UnityEngine.Input.GetKeyUp(_draw)) OnDrawReleased?.Invoke();
            if(UnityEngine.Input.GetKeyDown(_seekLockToggle)) OnSeekLockToggle?.Invoke();
            if(UnityEngine.Input.GetKeyDown(_togglePause)) OnTogglePause?.Invoke();
            Shift = UnityEngine.Input.GetKey(_shift);
        }
    }
}