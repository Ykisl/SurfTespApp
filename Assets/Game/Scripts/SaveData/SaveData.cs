using RedMoonGames.Basics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SaveData
{
    [Serializable]
    public class SaveData
    {
        [SerializeField] private Timestamp _saveTimestamp;
        [SerializeField] private List<SaveDataHandler> _dataHandlers = new List<SaveDataHandler>();

        public Timestamp? SaveTimestamp
        {
            get
            {
                if (_saveTimestamp.IsValidTimestamp)
                {
                    return _saveTimestamp;
                }

                return null;
            }

            set
            {
                _saveTimestamp = value.Value;
            }
        }

        public SaveDataHandler this[string targetId]
        {
            get
            {
                return GetDataHandler(targetId);
            }
        }

        public SaveDataHandler GetDataHandler(string targetId)
        {
            var dataHandler = _dataHandlers.GetBy(handler => handler.TargetId == targetId);

            if(dataHandler == null)
            {
                dataHandler = new SaveDataHandler
                {
                    JsonData = null,
                    TargetId = targetId,
                    Version = 0
                };
            }

            return dataHandler;
        }

        public void SetDataHandler(SaveDataHandler dataHandler)
        {
            if (_dataHandlers.Contains(dataHandler))
            {
                RemoveDataHandler(dataHandler);
            }

            if(dataHandler.JsonData == null)
            {
                return;
            }

            _dataHandlers.Add(dataHandler);
        }

        public void RemoveDataHandler(SaveDataHandler dataHandler)
        {
            RemoveDataHandler(dataHandler.TargetId);
        }

        public void RemoveDataHandler(string targetId)
        {
            _dataHandlers.RemoveAll(handler => handler.TargetId == targetId);
        }
    }
}
