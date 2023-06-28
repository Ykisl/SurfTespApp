using RedMoonGames.Database;
using UnityEngine;
using System;

namespace Game.Item.Settings
{
    [Serializable]
    public class ItemSettings : IDatabaseModelPrimaryKey<string>
    {
        public string Id;
        public string Name;
        public Vector2Int Size;
        public Sprite Icon;

        public string PrimaryKey => Id;
    }
}
