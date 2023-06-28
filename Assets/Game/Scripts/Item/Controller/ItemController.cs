using Game.Common;
using Game.Drag;
using Item.Model;
using RedMoonGames.Basics;
using UnityEngine;

namespace Game.Item.Controller
{
    public class ItemController : MonoBehaviour, IDraggable
    {
        [SerializeField] private ACommonView _view;
        [Space]
        [SerializeField] private RectTransform _itemTransform;
        [SerializeField] private float _slotSize = 50f;

        private ItemModel _model;

        public Vector2Int Size
        {
            get => _model.Size;
        }

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

        public TryResult TryStartDrag(Vector3 position)
        {
            return true;
        }

        public void StopDrag(Vector3 position, Vector3 startPosition)
        {
            
        }

        private void UpdateItemSize(Vector2Int itemSize)
        {
            var itemSizeVector = (Vector2)itemSize;
            itemSizeVector *= _slotSize;

            _itemTransform.sizeDelta = itemSizeVector;
        }
    }
}
