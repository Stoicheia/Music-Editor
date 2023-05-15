using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DefaultNamespace.Input
{
    
    public class Keybinds : MonoBehaviour
    {
        [Serializable]
        public class RecordKey
        {
            public KeyCode Key;
            public int Lane;
            public bool Hold;
        }
        
        public static bool RecordKeybinds { get; set; }

        public static event Action OnDrawPressed, OnDrawReleased, OnSeekLockToggle, OnTogglePause, 
                OnUndoPressed, OnRedoPressed, OnRecordPressed;

        public static event Action<RecordKey> OnRecordKey, OnHoldKey, OnReleaseKey;
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
        [SerializeField] private KeyCode _record = KeyCode.R;
        [SerializeField] private List<RecordKey> _recordKeys;

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
            if (!RecordKeybinds) return;
            if (EventSystem.current.currentSelectedGameObject != null) return;
            
            Shift = UnityEngine.Input.GetKey(KeyCode.LeftShift) || 
                    UnityEngine.Input.GetKey(KeyCode.RightShift);
            Ctrl = UnityEngine.Input.GetKey(KeyCode.LeftControl) || 
                   UnityEngine.Input.GetKey(KeyCode.RightControl) ||
                   UnityEngine.Input.GetKey(KeyCode.LeftCommand) ||
                   UnityEngine.Input.GetKey(KeyCode.RightCommand);
            
            if(UnityEngine.Input.GetKeyDown(_draw)) OnDrawPressed?.Invoke();
            if(UnityEngine.Input.GetKeyUp(_draw)) OnDrawReleased?.Invoke();
            if(UnityEngine.Input.GetKeyDown(_seekLockToggle)) OnSeekLockToggle?.Invoke();
            if(UnityEngine.Input.GetKeyDown(_record)) OnRecordPressed?.Invoke();
            if(UnityEngine.Input.GetKeyDown(_togglePause)) OnTogglePause?.Invoke();
            foreach (var keyPair in _recordKeys)
            {
                if (UnityEngine.Input.GetKeyDown(keyPair.Key))
                {
                    OnRecordKey?.Invoke(keyPair);
                }
                if (UnityEngine.Input.GetKey(keyPair.Key))
                {
                    OnHoldKey?.Invoke(keyPair);
                }
                if (UnityEngine.Input.GetKeyUp(keyPair.Key))
                {
                    OnReleaseKey?.Invoke(keyPair);
                }
            }
            if (Ctrl)
            {
                if(UnityEngine.Input.GetKeyDown(_undo)) OnUndoPressed?.Invoke();
                if(UnityEngine.Input.GetKeyDown(_redo)) OnRedoPressed?.Invoke();
            }
        }
    }
}