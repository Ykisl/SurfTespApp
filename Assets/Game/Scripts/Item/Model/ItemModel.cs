using Game.Common;
using UnityEngine;
using System;

namespace Game.Item.Model
{
    [Serializable]
    public class ItemModel : ICommonModel
    {
        [SerializeField] private string _name;
        [SerializeField] private float _state;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private Vector2Int _size;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnModelChanged?.Invoke();
            }
        }

        public float State
        {
            get => _state;
            set
            {
                _state = value;
                OnModelChanged?.Invoke();
            }
        }

        public Sprite Sprite
        {
            get => _sprite;
            set
            {
                _sprite = value;
                OnModelChanged?.Invoke();
            }
        }

        public Vector2Int Size
        {
            get => _size;
        }

        public event Action OnModelChanged;

        public ItemModel(Vector2Int size)
        {
            _size = size;
        }
    }
}
