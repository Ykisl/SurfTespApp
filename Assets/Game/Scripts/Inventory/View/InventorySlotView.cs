using Game.Inventory.Model;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Common;

namespace Game.Inventory.View
{
    public class InventorySlotView : ACommonTypedView<InventorySlotModel>
    {
        [SerializeField] private Image _backgorund;
        [SerializeField] private TextMeshProUGUI _positionText;
        [Space]
        [SerializeField] private Color _emptyColor;
        [SerializeField] private Color _filledColor;

        protected override void UpdateView(InventorySlotModel model)
        {
            var slotColor = GetSlotColor(model.IsFilled);
            _backgorund.color = slotColor;

            _positionText.text = model.SlotPosition.ToString();
        }

        private Color GetSlotColor(bool isFilled)
        {
            if (isFilled)
            {
                return _filledColor;
            }

            return _emptyColor;
        }
    }
}
