using Rhythm;
using UnityEngine;

namespace UI
{
    public class NodeTimeEditor : NodePropertyDrawerUI
    {
        public override void DrawData()
        {
            base.DrawData();
            Data.PropertyName = "Time (sec)";
            _propertyNameField.text = "Time (sec)";
            _inputFields[0].text = (Data as FloatData)?.FloatValue.ToString();
        }
        
        protected override void OnChangePropertyName(string pName)
        {
            Data.PropertyName = "Time (sec)";
            _propertyNameField.text = "Time (sec)";
        }
        
        protected override char ValidateInput(string text, int charindex, char addedchar)
        {
            if (float.TryParse(text + addedchar, out float result))
                return addedchar;
            return '\0';
        }
        
        protected override void OnRequestValueChange(string s)
        {
            FloatData fData = Data as FloatData;
            if (fData == null)
            {
                Debug.LogError("Wrong data type for this property drawer");
                return;
            }
            if (float.TryParse(s, out float timeVal))
            {
                fData.FloatValue = timeVal;
            }
        }
    }
}