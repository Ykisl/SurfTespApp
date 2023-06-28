using UnityEngine;
using System;
using Game.Common;

namespace Game.Inventory.Model
{
    [Serializable]
    public class InventorySlotModel : ICommonModel
    {
        [SerializeField] private bool _isFilled;
        [SerializeField] private Vector2Int _slotPosition;

        public bool IsFilled
        {
            get => _isFilled;
            set
            {
                _isFilled = value;
                OnModelChanged?.Invoke();
            }
        }

        public Vector2Int SlotPosition
        {
            get => _slotPosition;
            set
            {
                _slotPosition = value;
                OnModelChanged?.Invoke();
            }
        }

        public event Action OnModelChanged;

        public InventorySlotModel(Vector2Int slotPosition)
        {
            _slotPosition = slotPosition;
        }
    }
}
