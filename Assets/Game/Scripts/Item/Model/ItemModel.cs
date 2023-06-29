using Game.Common;
using UnityEngine;
using System;

namespace Game.Item.Model
{
    [Serializable]
    public class ItemModel : ICommonModel
    {
        private string _id;
        private string _name;
        private float _state;
        private Sprite _sprite;
        private Vector2Int _size;

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnModelChanged?.Invoke();
            }
        }

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
