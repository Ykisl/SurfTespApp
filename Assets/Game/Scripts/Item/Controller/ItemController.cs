using Game.Common;
using Item.Model;
using UnityEngine;

namespace Item.Controller
{
    public class ItemController : MonoBehaviour
    {
        [SerializeField] private ACommonView _view;
        [Space]
        [SerializeField] private RectTransform _itemTransform;
        [SerializeField] private float _slotSize = 50f;

        private ItemModel _model;

        public void Init(ItemModel model)
        {
            _model = model;
            if (_model == null)
            {
                return;
            }

            _view.SetModel(_model);
            UpdateItemSize(_model.Size);
        }

        public void SetSlotSize(float slotSize)
        {
            _slotSize = slotSize;
            if(_model != null)
            {
                UpdateItemSize(_model.Size);
            }
        }

        private void UpdateItemSize(Vector2Int itemSize)
        {
            var itemSizeVector = (Vector2)itemSize;
            itemSizeVector *= _slotSize;

            _itemTransform.sizeDelta = itemSizeVector;
        }
    }
}
