using UnityEngine;
using RedMoonGames.Basics;
using Game.SaveData.Interfaces;
using System;

namespace Game.SaveData
{
    public class GameDataSaver : CachedBehaviour
    {
        [SerializeField] private string targetId = Guid.NewGuid().ToString();
        [SerializeField] private int version;
        [SerializeField] private int priority;
        [Space]
        [SerializeField] private CachedBehaviour target;

        private ISavable _savable;
        private ILoadFinished _loadFinished;

        public event Action<SaveDataHandler> OnSaveDataChanged;

        public string TargetId
        {
            get => targetId;
        }

        public int Priority 
        {
            get => priority;
        }

        private void Awake()
        {
            _savable = target.GetCachedComponent<ISavable>();
            _loadFinished = target.GetCachedComponent<ILoadFinished>();

            if(_savable != null)
            {
                _savable.OnSaveDataChanged += HandleSaveDataChanged;
            }
        }

        private void OnDestroy()
        {
            if (_savable != null)
            {
                _savable.OnSaveDataChanged -= HandleSaveDataChanged;
            }
        }

        public void LoadSaveData(SaveDataHandler saveDataHandler, Timestamp? saveTimestamp)
        {
            if(saveDataHandler.TargetId != targetId)
            {
                return;
            }

            _savable?.LoadSaveData(saveDataHandler.JsonData, saveDataHandler.Version, saveTimestamp);
        }

        public void LoadFinished(Timestamp? saveTimestamp)
        {
            _loadFinished.LoadFinished(saveTimestamp);
        }

        public SaveDataHandler GetSaveDataHandler()
        {
            var saveHandler = new SaveDataHandler
            {
                JsonData = _savable?.GetSaveData(),
                TargetId = targetId,
                Version = version
            };

            return saveHandler;
        }

        private void HandleSaveDataChanged()
        {
            var data = _savable?.GetSaveData();
            var saveHandler = GetSaveDataHandler();

            OnSaveDataChanged?.Invoke(saveHandler);
        }
    }
}
