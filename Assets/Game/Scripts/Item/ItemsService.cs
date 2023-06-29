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
            itemModel = null;
            itemController = null;

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
            itemController.Init(itemModel);

            _itemModels.Add(itemController, itemModel);
            return true;
        }

        public void RemoveItem(ItemController item)
        {
            if (!_itemModels.ContainsKey(item))
            {
                return;
            }

            _itemModels.Remove(item);
            Destroy(item);
        }

        public bool IsValidItemId(string itemId)
        {
            return _itemSettingsDatabase.GetItemSettingsById(itemId) != null;
        }
    }
}
