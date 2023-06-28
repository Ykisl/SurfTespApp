using Game.Common;
using Game.Inventory.Model;
using UnityEngine;

namespace Game.Inventory.Controller
{
    public class InventorySlotController : MonoBehaviour
    {
        [SerializeField] private ACommonView _view;

        private InventorySlotModel _model;

        public void Init(InventorySlotModel model)
        {
            _model = model;
            if(_model == null)
            {
                return;
            }

            _view.SetModel(_model);
        }
    }
}
