using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Common;
using Game.Item.Model;

namespace Game.Item.View
{
    public class ItemView : ACommonTypedView<ItemModel>
    {
        [SerializeField] private Image _spriteImage;
        [SerializeField] private TextMeshProUGUI _stateText;

        protected override void UpdateView(ItemModel model)
        {
            _spriteImage.sprite = model.Sprite;
            _spriteImage.preserveAspect = true;

            _stateText.text = model.State.ToString();
        }
    }
}
