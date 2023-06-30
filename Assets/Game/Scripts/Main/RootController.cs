using UnityEngine;
using Game.Inventory.Controller;
using Game.Inventory.Model;
using Game.Item.Settings;
using Game.Item.Model;
using System.Collections.Generic;
using System;
using RedMoonGames.Basics;
using Game.SaveData.Interfaces;
using System.Linq;
using Game.Item.Controller;
using Game.Item;

namespace Game.Main
{
    [Serializable]
    public class RootControllerData
    {
        public InventoryControllerData InventoryData;
    }
    public class RootController : CachedBehaviour, ISavable, ILoadFinished
    {
        [SerializeField] private float _slotSize = 50f;
        [SerializeField] private Vector2Int _mainInventorySize;
        [SerializeField] private Vector2Int _itemsListInventorySize;
        [SerializeField] private int _randomItemsTargetAmount;
        [Space]
        [SerializeField] private InventoryController _mainInventory;
        [SerializeField] private InventoryController _itemsListInventory;
        [Space]
        [SerializeField] private ItemsService _itemsService;

        private InventoryModel _mainInventoryModel;
        private InventoryModel _itemsListInventoryModel;

        private RootControllerData _data;
        private Dictionary<ItemModel, ItemData> _items = new Dictionary<ItemModel, ItemData>();

        public event Action OnSaveDataChanged;

        private void Init()
        {
            _mainInventoryModel = new InventoryModel(_mainInventorySize, _slotSize);
            _mainInventory.Init(_mainInventoryModel);

            _itemsListInventoryModel = new InventoryModel(_itemsListInventorySize, _slotSize);
            _itemsListInventory.Init(_itemsListInventoryModel);

            _mainInventory.OnDataChanged += HandleInventoryDataChanged;
            _mainInventory.OnItemTapped += HandleInventoryItemTapped;
        }

        private void OnDestroy()
        {
            _mainInventory.OnDataChanged -= HandleInventoryDataChanged;
            _mainInventory.OnItemTapped -= HandleInventoryItemTapped;
        }

        public void LoadFinished(Timestamp? saveTimestamp)
        {
            GenerateRandomItemsToInventory(_itemsListInventory, _randomItemsTargetAmount);
        }

        public string GetSaveData()
        {
            return JsonUtility.ToJson(_data);
        }

        public void LoadSaveData(string saveData, int saveVersion, Timestamp? saveTimestamp)
        {
            _data = JsonUtility.FromJson<RootControllerData>(saveData) ?? new RootControllerData();

            Init();
            _mainInventory.Load(_data.InventoryData);
        }

        private void GenerateRandomItemsToInventory(InventoryController inventory, int targetItemsCount)
        {
            for(int i = 0; i < targetItemsCount; i++)
            {
                if(!_itemsService.TryCreateRandomItem(out var itemModel, out var itemController))
                {
                    return;
                }

                if(!inventory.TryGetAvalibleSlotPositionForSize(itemController.Size, out var slotPositon))
                {
                    _itemsService.RemoveItem(itemController);
                    continue;
                }

                if(!inventory.TryAddItemController(itemController, slotPositon))
                {
                    _itemsService.RemoveItem(itemController);
                    continue;
                }
            }
        }

        private void HandleInventoryDataChanged(InventoryControllerData inventoryControllerData)
        {
            OnSaveDataChanged?.Invoke();
        }

        private void HandleInventoryItemTapped(InventoryController inventory, ItemController item)
        {
            if(!_itemsListInventoryModel.TryGetAvalibleSlotPositionForSize(item.Size, out var avalibleSlot))
            {
                return;
            }

            inventory.RemoveItemController(item);
            _itemsListInventory.TryAddItemController(item, avalibleSlot);
        }
    }
}
