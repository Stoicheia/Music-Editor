using System;
using Rhythm;
using RhythmEngine;
using TMPro;
using UnityEngine;
using Utility;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class TimeSignatureField : MonoBehaviour
    {
        public TimeSignatureChange ChangeFlag { get; set; }
        [NonSerialized] public RectTransform rectTransform;
        
        [SerializeField] private TMP_InputField _numeratorField;
        [SerializeField] private TMP_InputField _denominatorField;
        
        private void Awake()
        {
            _numeratorField.onValidateInput += ValidateInput;
            _numeratorField.onValueChanged.AddListener(HandleValueChange);
            _denominatorField.onValidateInput += ValidateInput;
            _denominatorField.onValueChanged.AddListener(HandleValueChange);

            rectTransform = GetComponent<RectTransform>();
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
            int numerator = 4;
            int denominator = 4;
            int.TryParse(_numeratorField.text, out numerator);
            int.TryParse(_denominatorField.text, out denominator);
            
           ChangeFlag.TimeSignature = new TimeSignature(numerator, denominator);
        }

        public void SetText(string s1, string s2)
        {
            _numeratorField.text = s1;
            _denominatorField.text = s2;
        }

    }
}