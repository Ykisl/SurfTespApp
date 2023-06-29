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
        [Space]
        [SerializeField] private InventoryController _mainInventory;
        [SerializeField] private InventoryController _itemsListInventory;

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
        }

        public void LoadFinished(Timestamp? saveTimestamp)
        {
            
        }

        public string GetSaveData()
        {
            return JsonUtility.ToJson(_data);
        }

        public void LoadSaveData(string saveData, int saveVersion, Timestamp? saveTimestamp)
        {
            _data = JsonUtility.FromJson<RootControllerData>(saveData) ?? new RootControllerData()
            {
                InventoryData = new InventoryControllerData()
                {
                    Items = new UnityDictionary<ItemData, Vector2Int>()
                    {
                        { new ItemData(){ItemId = "Item02", State = 1f}, new Vector2Int(2,2) },
                        { new ItemData(){ItemId = "Item01", State = 0f}, new Vector2Int(5,5) }
                    }
                }
            };

            Init();
            _mainInventory.Load(_data.InventoryData);
        }
    }
}
