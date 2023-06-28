using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedMoonGames.Basics;
using UnityEngine.EventSystems;
using System.Linq;

namespace Game.Drag
{
    public class DragService : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        private IDraggable _currentDraggableElement;
        private Transform _currentDraggableTransform;
        private Vector3 _dragStartPosition;

        private void Update()
        {
            var mousePosition = Input.mousePosition;
            var screenPosition = _camera.ScreenToViewportPoint(mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                TryStartDrag(mousePosition, screenPosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                StopDrag(screenPosition);
            }

            if(_currentDraggableTransform != null)
            {
                _currentDraggableTransform.position = mousePosition;
            }
        }

        private TryResult TryStartDrag(Vector3 position, Vector3 screenPosition)
        {
            if(_currentDraggableElement != null)
            {
                return false;
            }

            if(!TryGetDraggableElements(position, out var draggableElements))
            {
                return false;
            }

            var draggableElement = draggableElements.FirstOrDefault();
            if (!draggableElement.TryStartDrag(screenPosition))
            {
                return false;
            }

            _currentDraggableElement = draggableElement;
            if(_currentDraggableElement is MonoBehaviour mono)
            {
                _currentDraggableTransform = mono.transform;
                _dragStartPosition = _currentDraggableTransform.position;
            }

            return true;
        }

        private void StopDrag(Vector3 screenPosition)
        {
            if (_currentDraggableElement == null)
            {
                return;
            }

            _currentDraggableElement.StopDrag(screenPosition, _dragStartPosition);

            _currentDraggableTransform = null;
            _currentDraggableElement = null;
        }

        private TryResult TryGetDraggableElements(Vector3 position, out ICollection<IDraggable> draggableElements)
        {
            var pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = position
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);
            if (results.Count <= 0)
            {
                draggableElements = null;
                return false;
            }

            draggableElements = results
                .Where(x => x.gameObject.TryGetComponent<IDraggable>(out var draggable))
                .Select(x => x.gameObject.GetComponent<IDraggable>()).ToList();

            return draggableElements.Count > 0;
        }
    }
}
