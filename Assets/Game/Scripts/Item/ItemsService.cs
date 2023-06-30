using Game.Item.Settings;
using UnityEngine;
using RedMoonGames.Basics;
using Game.Item.Model;
using Game.Item.Controller;
using System.Collections.Generic;

namespace Game.Item
{
    public class ItemsService : MonoBehaviour
    {
        [SerializeField] private ItemSettingsDatabase _itemSettingsDatabase;
        [Space]
        [SerializeField] private ItemController _itemPrefab;

        private Dictionary<ItemController, ItemModel> _itemModels = new Dictionary<ItemController, ItemModel>();

        public TryResult TryCreateItem(string itemId, out ItemModel itemModel, out ItemController itemController)
        {
            var itemData = new ItemData
            {
                ItemId = itemId
            };

            return TryCreateItem(itemData, out itemModel, out itemController);
        }

        public TryResult TryCreateItem(ItemData itemData, out ItemModel itemModel, out ItemController itemController)
        {
            itemModel = null;
            itemController = null;

            var itemId = itemData.ItemId;
            var itemSettings = _itemSettingsDatabase.GetItemSettingsById(itemId);
            if (itemSettings == null)
            {
                return false;
            }

            itemModel = new ItemModel(itemSettings.Size)
            {
                Id = itemSettings.Id,
                Name = itemSettings.Name,
                Sprite = itemSettings.Icon,
            };

            itemController = Instantiate(_itemPrefab);
            itemController.Init(itemModel, itemData);

            _itemModels.Add(itemController, itemModel);
            return true;
        }

        public TryResult TryCreateRandomItem(out ItemModel itemModel, out ItemController itemController)
        {
            var itemSettings = _itemSettingsDatabase.GetData();
            var randomItemSetting = itemSettings.GetRandom();
            if(randomItemSetting == null)
            {
                itemModel = null;
                itemController = null;
                return false;
            }

            return TryCreateItem(randomItemSetting.Id, out itemModel, out itemController);
        }

        public void RemoveItem(ItemController item)
        {
            if (!_itemModels.ContainsKey(item))
            {
                return;
            }

            _itemModels.Remove(item);
            Destroy(item.gameObject);
        }

        public bool IsValidItemId(string itemId)
        {
            return _itemSettingsDatabase.GetItemSettingsById(itemId) != null;
        }
    }
}
