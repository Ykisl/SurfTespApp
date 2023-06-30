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
        [SerializeField] private Transform _draggableRoot;
        [Space]
        [SerializeField] private float _maxDistanceForTap = 0.05f;

        private IDraggable _currentDraggableElement;
        private Transform _currentDraggableTransform;
        private Vector3 _dragStartPosition;
        private Vector3 _dragStartMousePosition;

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
                StopDrag(mousePosition, screenPosition);
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

            if(!TryGetElementsInPosition<IDraggable>(position, out var draggableElements))
            {
                return false;
            }

            var draggableElement = draggableElements.FirstOrDefault();
            if (!draggableElement.TryStartDrag(screenPosition))
            {
                return false;
            }

            _dragStartMousePosition = position;
            _currentDraggableElement = draggableElement;
            if(_currentDraggableElement is MonoBehaviour mono)
            {
                _currentDraggableTransform = mono.transform;
                _dragStartPosition = _currentDraggableTransform.position;

                _currentDraggableTransform.SetParent(_draggableRoot);
            }

            return true;
        }

        private void StopDrag(Vector3 position, Vector3 screenPosition)
        {
            if (_currentDraggableElement == null)
            {
                return;
            }

            var dragDistance = Vector2.Distance(_dragStartMousePosition, position);
            if(dragDistance <= _maxDistanceForTap)
            {
                _currentDraggableElement.Tap(position);
            }
            else
            {
                TryGetElementsInPosition<IDragTarget>(position, out var dragTargets);
                var validDragTargets = dragTargets?.Where(target => target.IsValdDraggable(_currentDraggableElement));
                var dragTarget = validDragTargets?.FirstOrDefault();

                _currentDraggableElement.StopDrag(dragTarget, position, _dragStartPosition, dragDistance);
                dragTarget?.DropDraggable(_currentDraggableElement);
            }

            _currentDraggableTransform = null;
            _currentDraggableTransform = null;
            _currentDraggableElement = null;
        }

        private TryResult TryGetElementsInPosition<TElementType>(Vector3 position, out ICollection<TElementType> elements)
        {
            var pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = position
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);
            if (results.Count <= 0)
            {
                elements = null;
                return false;
            }

            elements = results
                .Select(x => 
                {
                    x.gameObject.TryGetComponent<TElementType>(out var element);
                    return element;
                })
                .Where(x => x != null).ToList();

            return elements.Count > 0;
        }
    }
}
