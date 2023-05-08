using System;
using System.Collections.Generic;
using Rhythm;
using UnityEditor.U2D.Path;
using UnityEngine;

namespace UI
{
    public class NodeDataEditorUI : MonoBehaviour
    {
        public RhythmEvent ActiveEvent
        {
            get
            {
                return _activeEvent;
            }
            set
            {
                _activeEvent = value;
                if(value == null) _graphicsRoot.gameObject.SetActive(false);
                else UpdateGraphics(_activeEvent);
            }
        }

        [Header("Graphics Options")]
        [SerializeField] private NodeIntDrawerUI _nodeIntDrawerPrefab;
        [SerializeField] private NodeStringDrawerUI _nodeStringDrawerPrefab;
        [Header("Advanced")] 
        [SerializeField] private RectTransform _graphicsRoot;

        private RhythmEvent _activeEvent;

        private List<NodePropertyDrawerUI> _propertyDrawers;

        private void Awake()
        {
            _propertyDrawers = new List<NodePropertyDrawerUI>();
        }

        private void UpdateGraphics(RhythmEvent e)
        {
            // Clear all property drawers
            foreach (var p in _propertyDrawers)
            {
                Destroy(p.gameObject);
            }
            _propertyDrawers.Clear();
            
            // Draw properties
            List<RhythmData> properties = e.RhythmData;
            foreach (var p in properties)
            {
                if (p.IsType<IntData>())
                {
                    NodeIntDrawerUI drawer = Instantiate(_nodeIntDrawerPrefab, _graphicsRoot);
                    // etc
                }
                else if (p.IsType<StringData>())
                {
                    NodeStringDrawerUI drawer = Instantiate(_nodeStringDrawerPrefab, _graphicsRoot);
                    // etc
                }
            }
        }
    }
}