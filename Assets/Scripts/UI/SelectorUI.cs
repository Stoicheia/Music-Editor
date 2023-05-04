using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class SelectorUI : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler, IPointerUpHandler, IPointerClickHandler
    {
        [SerializeField] private float _doubleClickThreshold;
        
        private ISelectorInteractor _currentSelectedObject;
        private SelectInfo _currentSelectedInfo;
        
        private float _doubleClickTimer;
        
        public Vector2 MouseScreenPosition { get; private set; }

        private void Update()
        {
            if(_currentSelectedObject != null) MoveObject(_currentSelectedObject, MouseScreenPosition);
            _doubleClickTimer -= Time.deltaTime;
        }

        public void SelectObject(ISelectorInteractor isl, SelectInfo info, Vector2 pos, bool doubleClick = false)
        {
            isl.Select(info, pos, doubleClick);
        }

        public void ClickOnObject(ISelectorInteractor isl, SelectInfo info, Vector2 pos)
        {
            _currentSelectedObject = isl;
            _currentSelectedInfo = info;
            isl.Click(info, pos);
        }

        public void PlaceObject(ISelectorInteractor isl, Vector2 pos)
        {
            isl.Place(_currentSelectedInfo, pos);
            _currentSelectedInfo = new SelectInfo(null);
            _currentSelectedObject = null;
        }

        public void PassRightClickToObject(ISelectorInteractor isl, Vector2 pos)
        {
            isl.RightClicked(_currentSelectedInfo, pos);
        }

        private void MoveObject(ISelectorInteractor isl, Vector2 pos)
        {
            if(isl != null)
                isl.Move(_currentSelectedInfo, pos);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                foreach (var r in results)
                {
                    ISelectorInteractor interacted = r.gameObject.GetComponent<ISelectorInteractor>();
                    if (interacted == null) continue;
                    SelectObject(interacted, new (), eventData.pointerCurrentRaycast.screenPosition,
                        _doubleClickTimer > 0);
                    break;
                }

                _doubleClickTimer = _doubleClickThreshold;
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                foreach (var r in results)
                {
                    ISelectorInteractor interacted = r.gameObject.GetComponent<ISelectorInteractor>();
                    if (interacted == null) continue;
                    PassRightClickToObject(interacted, eventData.pointerCurrentRaycast.screenPosition);
                    break;
                }
            }
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            MouseScreenPosition = eventData.position;
         }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(_currentSelectedInfo.Selected != null)
                PlaceObject(_currentSelectedObject, MouseScreenPosition);   
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                foreach (var r in results)
                {
                    ISelectorInteractor interacted = r.gameObject.GetComponent<ISelectorInteractor>();
                    if (interacted == null) continue;
                    ClickOnObject(interacted, new SelectInfo(interacted), eventData.pointerCurrentRaycast.screenPosition);
                    break;
                }
            }
        }
    }
}