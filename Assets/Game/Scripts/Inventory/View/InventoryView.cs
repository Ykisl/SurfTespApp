using UnityEngine;
using UnityEngine.UI;

namespace Game.Inventory.View
{
    public class InventoryView : ACommonTypedView<InventoryModel>
    {
        [SerializeField] private GridLayoutGroup _grid;

        protected override void UpdateView(InventoryModel model)
        {
            _grid.cellSize = new Vector2(model.SlotSize, model.SlotSize);
            SetGridSize(model.Size);
        }

        private void SetGridSize(Vector2Int size)
        {
            _grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            _grid.constraintCount = size.x;
        }
    }
}
