using Game.Common;
using Game.Drag;
using Game.Item.Model;
using RedMoonGames.Basics;
using System;
using UnityEngine;

namespace Game.Item.Controller
{
    [Serializable]
    public class ItemData
    {
        public string ItemId;
        public float State;
    }
    public class ItemController : MonoBehaviour, IDraggable
    {
        [SerializeField] private ACommonView _view;
        [Space]
        [SerializeField] private RectTransform _itemTransform;
        [SerializeField] private RectTransform _itemPositionTransform;
        [SerializeField] private float _slotSize = 50f;

        private ItemModel _model;
        private ItemData _data;

        public Vector2Int Size
        {
            get => _model.Size;
        }

        public Vector2 ItemPosition
        {
            get => ToItemPosition(_itemTransform.position);
        }

        public ItemModel Model
        {
            get => _model;
        }

        public ItemData Data
        {
            get => _data;
        }

        public event Action<ItemController, Vector2> OnDragStarted;
        public event Action<ItemController, Vector2, Vector2> OnDragDiscarded;
        public event Action<ItemController, Vector2> OnDragFinished;
        public event Action<ItemController, Vector2> OnTapped;

        public void Init(ItemModel model, ItemData itemData)
        {
            _data = itemData ?? new ItemData();

            _model = model;
            if (_model == null)
            {
                return;
            }

            _view.SetModel(_model);
            UpdateItemSize(_model.Size);
        }

        public void SetSlotSize(float slotSize)
        {
            _slotSize = slotSize;

            _itemPositionTransform.sizeDelta = Vector2.one * _slotSize;

            var halfSlotSize = _slotSize / 2;
            _itemPositionTransform.anchoredPosition = new Vector2(halfSlotSize, -halfSlotSize);

            if (_model != null)
            {
                UpdateItemSize(_model.Size);
            }
        }

        public TryResult TryStartDrag(Vector3 position)
        {
            OnDragStarted?.Invoke(this, ToItemPosition(position));
            return true;
        }

        public void Tap(Vector3 position)
        {
            OnDragDiscarded?.Invoke(this, position, ToItemPosition(position));
            OnTapped?.Invoke(this, position);
        }

        public void StopDrag(IDragTarget dragTarget, Vector3 position, Vector3 startPosition, float dragDistance)
        {
            var itemPosition = ToItemPosition(position);
            if (dragTarget == null)
            {
                OnDragDiscarded?.Invoke(this, itemPosition, ToItemPosition(startPosition));
                return;
            }

            OnDragFinished?.Invoke(this, itemPosition);
        }

        private void UpdateItemSize(Vector2Int itemSize)
        {
            var itemSizeVector = (Vector2)itemSize;
            itemSizeVector *= _slotSize;

            _itemTransform.localScale = Vector3.one;
            _itemTransform.sizeDelta = itemSizeVector;
        }

        private Vector2 ToItemPosition(Vector2 position)
        {
            var centerPosition = (Vector2)_itemTransform.position;
            var delta = centerPosition - (Vector2)_itemPositionTransform.position;
            return position - delta;
        }
    }
}
