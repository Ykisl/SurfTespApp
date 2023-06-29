using Game.Common;
using Game.Inventory.Model;
using Game.Item;
using Game.Item.Controller;
using Game.Item.Model;
using RedMoonGames.Basics;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Inventory.Controller
{
    [Serializable]
    public class InventoryControllerData
    {
        public UnityDictionary<ItemData, Vector2Int> Items = new UnityDictionary<ItemData, Vector2Int>();
    }
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private ACommonView _view;
        [Space]
        [SerializeField] private RectTransform _gridRoot;
        [SerializeField] private InventorySlotController _slotPrefab;
        [Space]
        [SerializeField] private RectTransform _itemsRoot;
        [Space]
        [SerializeField] private ItemsService _itemsService;

        private InventoryModel _model;
        private InventoryControllerData _data = new InventoryControllerData();

        private Pool<InventorySlotController> _slotsPool;
        private Dictionary<InventorySlotModel, InventorySlotController> _slotControllers;

        public event Action<InventoryController, ItemData, Vector2Int> OnItemLoad;

        private void Awake()
        {
            _slotsPool = new Pool<InventorySlotController>(() =>
            {
                var slotController = Instantiate(_slotPrefab);
                slotController.transform.SetParent(_gridRoot);

                return slotController;
            }, 10);

            _slotControllers = new Dictionary<InventorySlotModel, InventorySlotController>();
        }

        public void Init(InventoryModel model)
        {
            _model = model;
            if(_model == null)
            {
                return;
            }

            _view.SetModel(_model);
            UpdateSlotControllers(_model);
        }

        public void Load(InventoryControllerData data)
        {
            _data = data ?? new InventoryControllerData();

            foreach (var itemPair in _data.Items)
            {
                var itemData = itemPair.Key;
                if (!_itemsService.TryCreateItem(itemData.ItemId, out var itemModel, out var itemController))
                {
                    continue;
                }

                itemModel.State = itemData.State;
                var itemPosition = itemPair.Value;

                if (!_model.TryAddItem(itemPosition, itemModel))
                {
                    _itemsService.RemoveItem(itemController);
                    continue;
                }

                MoveItemControllerToSlot(itemController, itemPosition);
            }
        }

        public Vector2 GetSlotWorldPosition(Vector2Int slotPosition)
        {
            if(_model == null)
            {
                return Vector2.zero;
            }

            var slot = _model.GetSlot(slotPosition);
            if(slot == null || !_slotControllers.ContainsKey(slot))
            {
                return Vector2.zero;
            }

            var slotController = _slotControllers[slot];
            return slotController.transform.position;
        }

        public bool IsSlotBoxEmpty(Vector2Int slotPosition, Vector2Int slotSize)
        {
            return _model.TryGetEmptySlotsForSize(slotPosition, slotSize, out var slots);
        }

        private void UpdateSlotControllers(InventoryModel newModel)
        {
            ClearSlotsControllers();

            var slots = newModel.Slots;
            foreach(var slot in slots.Values)
            {
                var slotController = _slotsPool.Take();

                var slotPosition = slot.SlotPosition;
                var slotIndex = slotPosition.y * newModel.Size.x + slotPosition.x;
                slotController.transform.SetSiblingIndex(slotIndex);

                slotController.Init(slot);
                slotController.gameObject.SetActive(true);

                _slotControllers.Add(slot, slotController);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_gridRoot);
        }

        private void ClearSlotsControllers()
        {
            _slotControllers.Clear();

            foreach (var slot in _slotsPool.GetActive())
            {
                _slotsPool.Recycle(slot);
                slot.gameObject.SetActive(false);
            }
        }

        private void MoveItemControllerToSlot(ItemController itemController, Vector2Int slotPosition)
        {
            Debug.Log(_model);
            var sideSlotPosition = _model.GetBoxSideSlotPosition(slotPosition, itemController.Size);

            var mainPosition = GetSlotWorldPosition(slotPosition);
            var sidePosition = GetSlotWorldPosition(sideSlotPosition);

            var worldPosition = (mainPosition + sidePosition) / 2;

            var itemTransform = itemController.transform;
            itemTransform.SetParent(_itemsRoot);
            itemTransform.position = worldPosition;

            itemController.SetSlotSize(_model.SlotSize);
        }
    }
}
