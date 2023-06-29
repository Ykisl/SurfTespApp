using System;

namespace Game.SaveData
{
    [Serializable]
    public class SaveDataHandler
    {
        public string TargetId;
        public string JsonData;
        public int Version;

        public override bool Equals(object obj)
        {
            if(obj is null)
            {
                return false;
            }

            if(obj is not SaveDataHandler dataHandler)
            {
                return false;
            }

            return dataHandler.TargetId == TargetId;
        }

        public override int GetHashCode()
        {
            return TargetId.GetHashCode();
        }
    }
}
