using RedMoonGames.Basics;

namespace Game.SaveData.Interfaces
{
    public interface ILoadFinished
    {
        void LoadFinished(Timestamp? saveTimestamp);
    }
}
