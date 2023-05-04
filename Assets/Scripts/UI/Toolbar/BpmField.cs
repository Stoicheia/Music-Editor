using System;
using System.Collections.Generic;
using RhythmEngine;
using TMPro;
using UnityEngine;
using Utility;

namespace UI
{
    [RequireComponent( typeof(RectTransform))]
    public class BpmField : MonoBehaviour
    {
        public BpmChange ChangeFlag { get; set; }
        [NonSerialized] public RectTransform rectTransform;
        
        [SerializeField] private TMP_InputField _inputField;
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
    }
}