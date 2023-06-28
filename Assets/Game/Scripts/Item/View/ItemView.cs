using Item.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Item.View
{
    public class ItemView : ACommonTypedView<ItemModel>
    {
        [SerializeField] private Image _spriteImage;

        protected override void UpdateView(ItemModel model)
        {
            _spriteImage.sprite = model.Sprite;
            _spriteImage.preserveAspect = true;
        }
    }
}
