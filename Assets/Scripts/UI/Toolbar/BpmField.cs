using System;
using System.Collections.Generic;
using RhythmEngine;
using TMPro;
using UnityEngine;
using Utility;

namespace UI
{
    [RequireComponent( typeof(RectTransform))]
    public class BpmField : MonoBehaviour, ISelectorInteractor
    {
        public static event Action<BpmChange> OnRequestDelete;
        
        public BpmChange ChangeFlag { get; set; }

        public bool Lock
        {
            get => _lock;
            set
            {
                _lock = value;
                _lockGraphics.gameObject.SetActive(_lock);
            }
        }
        [NonSerialized] public RectTransform rectTransform;
        
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private RectTransform _lockGraphics;
        private bool _lock;
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            _inputField.onValidateInput += ValidateInput;
            _inputField.onValueChanged.AddListener(HandleValueChange);
        }

        private char ValidateInput(string input, int index, char newChar)
        {
            if (!StringUtility.DIGITS.Contains(newChar))
            {
                newChar = '\0';
            }

            return newChar;
        }

        private void HandleValueChange(string s)
        {
            float bpm = 120;
            float.TryParse(s, out bpm);
            ChangeFlag.Bpm = bpm;
        }

        public void SetText(string s)
        {
            _inputField.text = s;
        }

        public void Select(SelectInfo info, Vector2 pos, bool special = false)
        {
            //throw new NotImplementedException();
        }

        public void Click(SelectInfo info, Vector2 pos)
        {
            //throw new NotImplementedException();
        }

        public void Move(SelectInfo info, Vector2 pos)
        {
            //throw new NotImplementedException();
        }

        public void Place(SelectInfo info, Vector2 pos)
        {
            //throw new NotImplementedException();
        }

        public void RightClicked(SelectInfo info, Vector2 pos)
        {
            OnRequestDelete?.Invoke(ChangeFlag);
            // TODO: Metadata!
        }
    }
}