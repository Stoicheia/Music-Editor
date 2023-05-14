using System;
using System.Collections.Generic;
using Rhythm;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public abstract class NodePropertyDrawerUI : MonoBehaviour
    {
        public event Action<NodePropertyDrawerUI> OnRequestRemove;
        public RhythmData Data { get; set; }
        
        [Header("Fields")]
        [SerializeField] protected TMP_InputField _propertyNameField;
        [SerializeField] protected List<TMP_InputField> _inputFields;

        [Header("Functional Objects")] 
        [SerializeField] protected Button _removeButton;
        
        protected virtual void OnEnable()
        {
            foreach (var field in _inputFields)
            {
                field.onValidateInput += ValidateInput;
                field.onValueChanged.AddListener(OnRequestValueChange);
                
            }
            _propertyNameField.onValueChanged.AddListener(OnChangePropertyName);
            if(_removeButton != null)
                _removeButton.onClick.AddListener(RemoveProperty);
        }

        protected virtual void OnDisable()
        {
            foreach (var field in _inputFields)
            {
                field.onValidateInput -= ValidateInput;
                field.onValueChanged.RemoveListener(OnRequestValueChange);
            }
            _propertyNameField.onValueChanged.RemoveListener(OnChangePropertyName);
            if(_removeButton != null)
                _removeButton.onClick.RemoveListener(RemoveProperty);
        }

        protected virtual void OnChangePropertyName(string pName)
        {
            Data.PropertyName = pName;
        }

        public virtual void DrawData()
        {
            _propertyNameField.text = Data.PropertyName;
        }

        protected virtual char ValidateInput(string text, int charindex, char addedchar)
        {
            return addedchar;
        }

        protected abstract void OnRequestValueChange(string s);
        
        private void RemoveProperty()
        {
            OnRequestRemove?.Invoke(this);
        }
    }
}