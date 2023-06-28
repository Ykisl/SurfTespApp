using RedMoonGames.Database;
using RedMoonGames.Basics;
using UnityEngine;

namespace Game.Item.Settings
{
    [CreateAssetMenu(fileName = "ItemSettingsDatabase", menuName = "[RMG] Scriptable/Item/ItemSettingsDatabase", order = 1)]
    public class ItemSettingsDatabase : ScriptableDatabase<ItemSettings>
    {
        public ItemSettings GetItemSettingsById(string id)
        {
            return _data.GetBy(x => x.Id == id);
        }
    }
}
