using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UserInput
{
    public class RightClickDragDeleteManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
    {
        public static List<ISelectionBoxInteractor> Interactors;
        
        [Header("Graphics Options")]
        [SerializeField] private RectTransform _selectionBoxPrefab;
        [SerializeField] private RectTransform _selectionBoxRoot;
        [SerializeField] private RectTransform _relativeGraphic;

        private MultiSelectionBox _selectionBox;
        private RectTransform _selectionBoxInstance;
        private Vector2 _firstClickPoint;

        private void Awake()
        {
            Interactors = new List<ISelectionBoxInteractor>();
        }

        private void Start()
        {
            _selectionBoxInstance = Instantiate(_selectionBoxPrefab, _selectionBoxRoot);
            _selectionBoxInstance.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_selectionBox == null) return;
            
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right) return;
            Vector2 anchoredMousePos = _relativeGraphic.transform.InverseTransformPoint(eventData.position);
            _selectionBox = new MultiSelectionBox(Interactors, anchoredMousePos, _relativeGraphic);
            _selectionBoxInstance.gameObject.SetActive(true);
            UpdateGraphics();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right) return;
            _selectionBox.Select();
            _selectionBox = null;
            _selectionBoxInstance.gameObject.SetActive(false);
        }
        
        public void OnPointerMove(PointerEventData eventData)
        {
            if (_selectionBox == null) return;
            UpdateBounds(eventData.position);
            _selectionBox.UpdateSelection();
            UpdateGraphics();
        }

        private void UpdateBounds(Vector2 mousePos)
        {
            _selectionBox.EndPoint = _relativeGraphic.transform.InverseTransformPoint(mousePos);
        }

        private void UpdateGraphics()
        {
            var bounds = _selectionBox.Bounds;
            _selectionBoxInstance.anchoredPosition = bounds.center;
            _selectionBoxInstance.sizeDelta = new Vector2(bounds.width, bounds.height);
        }
    }
}