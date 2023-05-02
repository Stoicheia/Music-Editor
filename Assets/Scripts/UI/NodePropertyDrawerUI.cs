using TMPro;
using UnityEngine;

namespace UI
{
    public abstract class NodePropertyDrawerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _propertyNameField;

        public virtual void OnRequestValueChange(string s)
        {
            
        }
    }
}