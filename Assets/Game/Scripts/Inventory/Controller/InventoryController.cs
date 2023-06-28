using Game.Common;
using Game.Inventory.Model;
using RedMoonGames.Basics;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Inventory.Controller
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private ACommonView _view;
        [Space]
        [SerializeField] private Transform _gridRoot;
        [SerializeField] private InventorySlotController _slotPrefab;

        private InventoryModel _model;
        private Pool<InventorySlotController> _slotsPool;

        private void Awake()
        {
            _slotsPool = new Pool<InventorySlotController>(() =>
            {
                var slotController = Instantiate(_slotPrefab);
                slotController.transform.SetParent(_gridRoot);

                return slotController;
            },
            10);

            var testModel = new InventoryModel(new Vector2Int(10, 10));
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
            ResetSlots(_model);
        }

        private void ResetSlots(InventoryModel newModel)
        {
            ClearSlots();

            var slots = newModel.Slots;
            foreach(var slot in slots)
            {
                var slotController = _slotsPool.Take();

                var slotPosition = slot.SlotPosition;
                var slotIndex = slotPosition.y * newModel.Size.x + slotPosition.x;
                slotController.transform.SetSiblingIndex(slotIndex);

                slotController.Init(slot);
                slotController.gameObject.SetActive(true);
            }
        }

        private void ClearSlots()
        {
            foreach(var slot in _slotsPool.GetActive())
            {
                _slotsPool.Recycle(slot);
                slot.gameObject.SetActive(false);
            }
        }
    }
}
