using System;
using System.Collections.Generic;
using System.Net;
using Rhythm;
using RhythmEngine;
using UnityEngine;
using UnityEngine.UI;

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
                if(value == null) _allGraphics.gameObject.SetActive(false);
                else UpdateGraphics(_activeEvent);
            }
        }

        [Header("Graphics Options")]
        [SerializeField] private NodeIntDrawerUI _nodeIntDrawerPrefab;
        [SerializeField] private NodeStringDrawerUI _nodeStringDrawerPrefab;
        [SerializeField] private NodeTimeEditor _nodeTimeDrawerPrefab;
        [SerializeField] private NodeVerticalEditor _nodeVerticalDrawerPrefab;
        
        [Header("Graphics")] 
        [SerializeField] private RectTransform _graphicsRoot;
        [SerializeField] private RectTransform _allGraphics;

        [Header("Functional Objects")] 
        [SerializeField] private Button _addPropertyButton;

        [Header("Dependencies")] [SerializeField]
        private ToolbarUI _toolbar;

        private RhythmEvent _activeEvent;

        private List<NodePropertyDrawerUI> _propertyDrawers;

        private NodeTimeEditor _activeTimeDrawer;
        private NodeVerticalEditor _activeVerticalDrawer;
        private List<NodePropertyDrawerUI> _activeMetadataDrawers;

        private void Awake()
        {
            _propertyDrawers = new List<NodePropertyDrawerUI>();
            _activeMetadataDrawers = new List<NodePropertyDrawerUI>();
        }

        private void Start()
        {
            SelectorUI.OnSelectObject += HandleSelectObject;
            _addPropertyButton.onClick.AddListener(AddPropertyToCurrent);
        }
        private void Update()
        {
            ReadValues();
        }

        private void ReadValues()
        {
            if (_activeTimeDrawer != null)
            {
                var timeData = _activeTimeDrawer.Data as FloatData;
                if (timeData != null)
                    ActiveEvent.SetTime(timeData.FloatValue);
            }

            if (ActiveEvent != null && _toolbar.ActiveOption != ToolbarOption.Properties)
            {
                ActiveEvent = null;
            }
        }
        
        private void AddPropertyToCurrent()
        {
            ActiveEvent.AddData(new StringData("", "New Property"));
            UpdateGraphics(ActiveEvent);
        }

        private void HandleSelectObject(ISelectorInteractor obj)
        {
            EventNodeBase node = obj as EventNodeBase;
            if (node == null || _toolbar.ActiveOption != ToolbarOption.Properties) return;
            ActiveEvent = node.Parent.Event;
        }

        private void UpdateGraphics(RhythmEvent e)
        {
            _allGraphics.gameObject.SetActive(true);
            // Clear all property drawers
            foreach (var p in _activeMetadataDrawers)
            {
                p.OnRequestRemove -= RemoveProperty;
                Destroy(p.gameObject);
            }
            
            if(_activeTimeDrawer != null)
                Destroy(_activeTimeDrawer.gameObject);
            if(_activeVerticalDrawer != null)
                Destroy(_activeVerticalDrawer.gameObject);
            _propertyDrawers.Clear();
            _activeMetadataDrawers.Clear();
            
            // Draw time field
            NodeTimeEditor timeDrawer = Instantiate(_nodeTimeDrawerPrefab, _graphicsRoot);
            timeDrawer.Data = new FloatData(ActiveEvent.TimeSeconds);
            _activeTimeDrawer = timeDrawer;
            timeDrawer.DrawData();

            // Draw properties
            List<RhythmData> properties = e.RhythmData;
            foreach (var p in properties)
            {
                if (p.IsType<IntData>())
                {
                    NodeIntDrawerUI drawer = Instantiate(_nodeIntDrawerPrefab, _graphicsRoot);
                    _activeMetadataDrawers.Add(drawer);
                    drawer.Data = p as IntData;
                    drawer.DrawData();
                    drawer.OnRequestRemove += RemoveProperty;
                }
                else if (p.IsType<StringData>())
                {
                    NodeStringDrawerUI drawer = Instantiate(_nodeStringDrawerPrefab, _graphicsRoot);
                    _activeMetadataDrawers.Add(drawer);
                    drawer.Data = p as StringData;
                    drawer.DrawData();
                    drawer.OnRequestRemove += RemoveProperty;
                }
            }
        }

        private void RemoveProperty(NodePropertyDrawerUI obj)
        {
            ActiveEvent.RemoveData(obj.Data);
            UpdateGraphics(ActiveEvent);
        }
    }
}