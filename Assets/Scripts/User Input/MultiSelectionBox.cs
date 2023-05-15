using System.Collections.Generic;
using UI;
using UnityEngine;

namespace UserInput
{
    public interface ISelectionBoxInteractor
    {
        public void MultiSelect();
        public Vector2 GetSelectionPoint();
    }
    
    public class MultiSelectionBox
    {
        public Vector2 StartPoint
        {
            get;
            set;
        }

        public Vector2 EndPoint
        {
            get => _endPoint;
            set
            {
                _endPoint = value;
                _bounds.center = Vector2.Lerp(_startPoint, _endPoint, 0.5f);
                _bounds.height = Mathf.Abs(_startPoint.y - _endPoint.y);
                _bounds.width = Mathf.Abs(_startPoint.x - _endPoint.x);
            }
        }

        public Rect Bounds => _bounds;
        
        private Rect _bounds;
        private Rect _worldBounds;
        private Vector2 _startPoint;
        private Vector2 _endPoint;
        private List<ISelectionBoxInteractor> _selectedObjects;
        private List<ISelectionBoxInteractor> _targets;
        private RectTransform _relativeGraphic;

        public MultiSelectionBox(List<ISelectionBoxInteractor> t, Vector2 start, RectTransform rel)
        {
            _targets = t;
            _selectedObjects = new List<ISelectionBoxInteractor>();
            _bounds = Rect.zero;
            _worldBounds = Rect.zero;
            _startPoint = _endPoint = start;
            _relativeGraphic = rel;
        }

        public void Select()
        {
            _selectedObjects.ForEach(x => x.MultiSelect());
        }

        public void UpdateSelection()
        {
            _worldBounds.center = _relativeGraphic.TransformPoint(_bounds.center);
            _worldBounds.max = _relativeGraphic.TransformPoint(_bounds.max);
            _worldBounds.min = _relativeGraphic.TransformPoint(_bounds.min);
            
            _selectedObjects.Clear();
            foreach (var target in _targets)
            {
                Vector2 pos = target.GetSelectionPoint();
                if (!_worldBounds.Contains(pos)) continue;
                _selectedObjects.Add(target);
            }
        }
    }
}