using System;
using Unity.VisualScripting;
using UnityEngine;

namespace UI
{
    public class SelectorUI : MonoBehaviour
    {
        private RectTransform _currentSelectedObject;
        private SelectInfo _currentSelectedInfo;

        private void Update()
        {
            if(_currentSelectedObject != null) UpdateSelectedPosition();
        }

        public void Select(GameObject prefab, SelectInfo info)
        {
            var go = Instantiate(prefab, transform, true);
            _currentSelectedObject = go.AddComponent<RectTransform>();
            _currentSelectedInfo = info;
        }

        public void PassSelectedToObject(ISelectorInteractor isl)
        {
            isl.Place(_currentSelectedInfo);
            _currentSelectedInfo = null;
            Destroy(_currentSelectedObject.gameObject);
            _currentSelectedObject = null;
        }

        private void UpdateSelectedPosition()
        {
            _currentSelectedObject.position = Input.mousePosition;
        }
    }
}