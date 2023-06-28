using Game.Common;
using Game.Inventory.Model;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class InventoryModel : ICommonModel
{
    [SerializeField] private Vector2Int _size;
    [SerializeField] private List<InventorySlotModel> _slots;

    public Vector2Int Size
    {
        get => _size;
    }

    public int SlotsCount
    {
        get => _size.x * _size.y;
    }

    public IReadOnlyCollection<InventorySlotModel> Slots
    {
        get => _slots;
    }

    public event Action OnModelChanged;

    public InventoryModel(Vector2Int size)
    {
        _size = size;

        _slots = new List<InventorySlotModel>();
        for(int i = 0; i < _size.y; i++)
        {
            for (int k = 0; k < _size.x; k++)
            {
                var slotPosition = new Vector2Int(k, i);
                var slotModel = new InventorySlotModel(slotPosition);
                _slots.Add(slotModel);
            }
        }
    }
}
