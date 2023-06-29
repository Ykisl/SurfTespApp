using System;
using RedMoonGames.Basics;

namespace Game.SaveData.Interfaces
{
    public interface ISavable
    {
        event Action OnSaveDataChanged;

        void LoadSaveData(string saveData, int saveVersion, Timestamp? saveTimestamp);
        string GetSaveData();
    }
}
