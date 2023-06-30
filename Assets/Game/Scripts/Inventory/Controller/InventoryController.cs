using Game.Common;
using Game.Drag;
using Game.Inventory.Model;
using Game.Item;
using Game.Item.Controller;
using Game.Item.Model;
using RedMoonGames.Basics;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Inventory.Controller
{
    [Serializable]
    public class InventoryControllerData
    {
        public UnityDictionary<ItemData, Vector2Int> Items = new UnityDictionary<ItemData, Vector2Int>();
    }
    public class InventoryController : MonoBehaviour, IDragTarget
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

        private Dictionary<ItemController, ItemModel> _itemModels;
        private Dictionary<ItemController, ItemData> _itemData;

        public event Action<InventoryControllerData> OnDataChanged;
        public event Action<InventoryController, ItemController> OnItemTapped;

        private void Awake()
        {
            _slotsPool = new Pool<InventorySlotController>(() =>
            {
                var slotController = Instantiate(_slotPrefab);
                slotController.transform.SetParent(_gridRoot);

                return slotController;
            }, 10);

            _slotControllers = new Dictionary<InventorySlotModel, InventorySlotController>();
            _itemModels = new Dictionary<ItemController, ItemModel>();
            _itemData = new Dictionary<ItemController, ItemData>();
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
                if (!_itemsService.TryCreateItem(itemData, out var itemModel, out var itemController))
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

                _itemData.Add(itemController, itemData);
                AddItemControllerAssign(itemController, itemModel);
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

        public List<Vector2Int> GetNearestSlotsForWorldPosition(Vector2 position)
        {
            var slots = _model.Slots.Select(slot => slot.Key).ToList();
            slots.Sort((s1, s2) =>
            {
                var firstWorldPosition = GetSlotWorldPosition(s1);
                var firstDistance = Vector2.Distance(firstWorldPosition, position);
                var secondWorldPosition = GetSlotWorldPosition(s2);
                var secondDistance = Vector2.Distance(secondWorldPosition, position);
                return firstDistance.CompareTo(secondDistance);
            });

            return slots;
        }

        public TryResult TryGetAvalibleSlotPositionForSize(Vector2Int slotSize, out Vector2Int slotPosition)
        {
            return _model.TryGetAvalibleSlotPositionForSize(slotSize, out slotPosition);
        }

        public bool IsSlotBoxEmpty(Vector2Int slotPosition, Vector2Int slotSize)
        {
            return _model.TryGetEmptySlotsForSize(slotPosition, slotSize, out var slots);
        }

        public bool IsValdDraggable(IDraggable draggable)
        {
            if(draggable is not ItemController itemController)
            {
                return false;
            }

            var nearestSlots = GetNearestSlotsForWorldPosition(itemController.ItemPosition);
            var firstSlot = nearestSlots.FirstOrDefault();

            return IsSlotBoxEmpty(firstSlot, itemController.Size);
        }

        public void DropDraggable(IDraggable draggable)
        {
            var itemController = draggable as ItemController;

            var nearestSlots = GetNearestSlotsForWorldPosition(itemController.ItemPosition);
            var firstSlot = nearestSlots.FirstOrDefault();

            TryAddItemController(itemController, firstSlot);
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

        #region ItemController

        public TryResult TryAddItemController(ItemController itemController, Vector2Int slotPosition)
        {
            if (_itemData.ContainsKey(itemController))
            {
                return false;
            }

            if (!_model.TryAddItem(slotPosition, itemController.Model))
            {
                return false;
            }

            var itemData = itemController.Data;
            _data.Items.Add(itemData, slotPosition);
            _itemData.Add(itemController, itemData);

            AddItemControllerAssign(itemController, itemController.Model);
            MoveItemControllerToSlot(itemController, slotPosition);

            OnDataChanged?.Invoke(_data);
            return true;
        }

        public void RemoveItemController(ItemController itemController)
        {
            RemoveItemControllerAssign(itemController);

            if (!_itemData.TryGetValue(itemController, out var itemData))
            {
                return;
            }

            _model.RemoveItem(itemController.Model);

            _itemData?.Remove(itemController);
            _data.Items.Remove(itemData);

            OnDataChanged?.Invoke(_data);
        }

        private void AddItemControllerAssign(ItemController itemController, ItemModel itemModel)
        {
            if (_itemModels.ContainsKey(itemController))
            {
                return;
            }

            _itemModels.Add(itemController, itemModel);
            itemController.OnDragStarted += HandleItemDragStarted;
            itemController.OnDragFinished += HandleDragFinished;
            itemController.OnDragDiscarded += HandleItemDragDiscarded;
            itemController.OnTapped += HandleItemTapped;
        }

        private void RemoveItemControllerAssign(ItemController itemController)
        {
            if (!_itemModels.ContainsKey(itemController))
            {
                return;
            }

            itemController.OnDragStarted -= HandleItemDragStarted;
            itemController.OnDragFinished -= HandleDragFinished;
            itemController.OnDragDiscarded -= HandleItemDragDiscarded;
            itemController.OnTapped -= HandleItemTapped;

            _itemModels.Remove(itemController);
        }

        private void MoveItemControllerToSlot(ItemController itemController, Vector2Int slotPosition)
        {
            var sideSlotPosition = _model.GetBoxSideSlotPosition(slotPosition, itemController.Size);

            var mainPosition = GetSlotWorldPosition(slotPosition);
            var sidePosition = GetSlotWorldPosition(sideSlotPosition);

            var worldPosition = (mainPosition + sidePosition) / 2;

            var itemTransform = itemController.transform;
            itemTransform.SetParent(_itemsRoot);
            itemTransform.position = worldPosition;

            itemController.SetSlotSize(_model.SlotSize);
        }

        private void HandleItemDragStarted(ItemController itemController, Vector2 position)
        {
            var itemModel = _itemModels[itemController];
            _model.RemoveItem(itemModel);
        }

        private void HandleDragFinished(ItemController itemController, Vector2 dragStopPosition)
        {
            RemoveItemController(itemController);
        }

        private void HandleItemDragDiscarded(ItemController itemController, Vector2 dragStopPosition, Vector2 dragStartPosition)
        {
            var nearestSlots = GetNearestSlotsForWorldPosition(dragStartPosition);
            var firstSlot = nearestSlots.FirstOrDefault();

            var itemModel = _itemModels[itemController];
            _model.TryAddItem(firstSlot, itemModel);

            MoveItemControllerToSlot(itemController, firstSlot);
        }

        private void HandleItemTapped(ItemController item, Vector2 position)
        {
            OnItemTapped?.Invoke(this, item);
        }

        #endregion
    }
}
