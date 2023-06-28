using Game.Common;
using Game.Inventory.Model;
using Game.Item.Controller;
using Game.Item.Model;
using RedMoonGames.Basics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Inventory.Controller
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private ACommonView _view;
        [Space]
        [SerializeField] private RectTransform _gridRoot;
        [SerializeField] private InventorySlotController _slotPrefab;
        [Space]
        [SerializeField] private RectTransform _itemsRoot;
        [SerializeField] private ItemController _itemControllerPrefab;
        [Space]
        [SerializeField] private float _slotSize = 50f;
        [SerializeField] private Sprite _testItemSpite;

        private InventoryModel _model;
        private Pool<InventorySlotController> _slotsPool;
        private Dictionary<InventorySlotModel, InventorySlotController> _slotControllers;
        private Dictionary<ItemModel, ItemController> _itemControllers;

        private void Awake()
        {
            _slotsPool = new Pool<InventorySlotController>(() =>
            {
                var slotController = Instantiate(_slotPrefab);
                slotController.transform.SetParent(_gridRoot);

                return slotController;
            }, 10);

            _slotControllers = new Dictionary<InventorySlotModel, InventorySlotController>();
            _itemControllers = new Dictionary<ItemModel, ItemController>();
        }

        private void Start()
        {
            var testModel = new InventoryModel(new Vector2Int(10, 10), _slotSize);
            testModel.TryAddItem(new Vector2Int(1, 3), new ItemModel(new Vector2Int(3, 3)) 
            {
                Sprite = _testItemSpite
            });

            Init(testModel);
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
            UpdateItemControllers(_model);
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

        private void UpdateItemControllers(InventoryModel newModel)
        {
            ClearItemControllers();

            var items = newModel.Items;
            foreach(var item in items)
            {
                CreateItemController(item.Key, item.Value);
            }
        }

        private ItemController CreateItemController(ItemModel item, Vector2Int slotPosition)
        {
            var itemController = Instantiate(_itemControllerPrefab);
            itemController.Init(item);

            MoveItemControllerToSlot(itemController, slotPosition);
            _itemControllers.Add(item, itemController);

            return itemController;
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

        private void ClearItemControllers()
        {
            foreach (var itemPair in _itemControllers)
            {
                var itemGameObject = itemPair.Value.gameObject;
                Destroy(itemGameObject);
            }

            _itemControllers.Clear();
        }
    }
}
