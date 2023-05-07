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
        public static event Action OnUndoPressed;
        public static event Action OnRedoPressed;
        public static bool Shift;
        public static bool Ctrl;
        
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
        [SerializeField] private KeyCode _undo = KeyCode.Z;
        [SerializeField] private KeyCode _redo = KeyCode.Y;
        
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
            Shift = UnityEngine.Input.GetKey(KeyCode.LeftShift) || 
                    UnityEngine.Input.GetKey(KeyCode.RightShift);
            Ctrl = UnityEngine.Input.GetKey(KeyCode.LeftControl) || 
                   UnityEngine.Input.GetKey(KeyCode.RightControl) ||
                   UnityEngine.Input.GetKey(KeyCode.LeftCommand) ||
                   UnityEngine.Input.GetKey(KeyCode.RightCommand);
            if(UnityEngine.Input.GetKeyDown(_draw)) OnDrawPressed?.Invoke();
            if(UnityEngine.Input.GetKeyUp(_draw)) OnDrawReleased?.Invoke();
            if(UnityEngine.Input.GetKeyDown(_seekLockToggle)) OnSeekLockToggle?.Invoke();
            if(UnityEngine.Input.GetKeyDown(_togglePause)) OnTogglePause?.Invoke();
            if (Ctrl)
            {
                if(UnityEngine.Input.GetKeyDown(_undo)) OnUndoPressed?.Invoke();
                if(UnityEngine.Input.GetKeyDown(_redo)) OnRedoPressed?.Invoke();
            }
        }
    }
}