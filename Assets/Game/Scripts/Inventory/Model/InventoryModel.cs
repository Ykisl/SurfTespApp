using Game.Common;
using Game.Inventory.Model;
using Item.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RedMoonGames.Basics;

[Serializable]
public class InventoryModel : ICommonModel
{
    [SerializeField] private Vector2Int _size;
    [SerializeField] private float _slotSize;
    [SerializeField] private UnityDictionary<Vector2Int, InventorySlotModel> _slots;
    [SerializeField] private UnityDictionary<Vector2Int, Vector2Int> _slotParents;
    [SerializeField] private UnityDictionary<ItemModel, Vector2Int> _items;

    public Vector2Int Size
    {
        get => _size;
    }

    public int SlotsCount
    {
        get => _size.x * _size.y;
    }

    public float SlotSize
    {
        get => _slotSize;
    }

    public IReadOnlyDictionary<Vector2Int, InventorySlotModel> Slots
    {
        get => _slots;
    }

    public IReadOnlyDictionary<ItemModel, Vector2Int> Items
    {
        get => _items;
    }

    public event Action OnModelChanged;
    public event Action OnItemsChanged;

    public InventoryModel(Vector2Int size, float slotSize)
    {
        _size = size;
        _slotSize = slotSize;

        _slots = new UnityDictionary<Vector2Int, InventorySlotModel>();
        _slotParents = new UnityDictionary<Vector2Int, Vector2Int>();
        _items = new UnityDictionary<ItemModel, Vector2Int>();

        for (int i = 0; i < _size.y; i++)
        {
            for (int k = 0; k < _size.x; k++)
            {
                var slotPosition = new Vector2Int(k, i);
                var slotModel = new InventorySlotModel(slotPosition);
                _slots.Add(slotPosition, slotModel);
            }
        }
    }

    #region Slots

    public InventorySlotModel GetSlot(Vector2Int slotPosition)
    {
        if (!_slots.ContainsKey(slotPosition))
        {
            return null;
        }

        return _slots[slotPosition];
    }

    public IReadOnlyCollection<InventorySlotModel> GetEmptySlots()
    {
        return Slots.Values.Where(x => !x.IsFilled).ToArray();
    }

    public TryResult TryGetEmptySlotsForSize(Vector2Int slot, Vector2Int size, out IReadOnlyCollection<InventorySlotModel> emptySlots)
    {
        if (!TryGetSlotsForSize(slot, size, out emptySlots))
        {
            return false;
        }

        return emptySlots.All(slot => !slot.IsFilled);
    }

    public Vector2Int GetBoxSideSlotPosition(Vector2Int slot, Vector2Int size)
    {
        var sideSlot = slot + size - Vector2Int.one;
        return sideSlot;
    }

    public TryResult TryGetSlotsForSize(Vector2Int slot, Vector2Int size, out IReadOnlyCollection<InventorySlotModel> slots)
    {
        var sizeSlots = new List<InventorySlotModel>();
        slots = sizeSlots;

        if (size.x == 0 || size.y == 0)
        {
            return false;
        }

        var sideSlot = slot + size - Vector2Int.one;
        if(!_slots.ContainsKey(slot) || !_slots.ContainsKey(sideSlot))
        {
            return false;
        }

        var boxSlots = _slots.Where((slotPair) =>
        {
            var position = slotPair.Key;
            var isPositionInBox = position.x >= slot.x
            && position.y >= slot.y
            && position.x <= sideSlot.x
            && position.y <= sideSlot.y;

            return isPositionInBox;
        })
        .Select(slotPair => slotPair.Value);

        sizeSlots.AddRange(boxSlots);
        return true;
    }

    #endregion

    #region Items

    public TryResult TryAddItem(Vector2Int slotPosition, ItemModel item)
    {
        if(item == null)
        {
            return false;
        }

        var size = item.Size;
        if(!TryGetEmptySlotsForSize(slotPosition, size, out var subSlots))
        {
            return false;
        }

        _items.Add(item, slotPosition);
        foreach(var subSlot in subSlots)
        {
            subSlot.IsFilled = true;
            _slotParents.Add(subSlot.SlotPosition, slotPosition);
        }

        return true;
    }

    #endregion

}
