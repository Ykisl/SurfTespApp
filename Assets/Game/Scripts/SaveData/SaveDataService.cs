using System.Collections.Generic;
using UnityEngine;
using RedMoonGames.Basics;
using System.IO;
using System.Text;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.SaveData
{
    public class SaveDataService : CachedBehaviour
    {
        private const string FILE_NAME = "save.dat";

        private SaveData _saveData = new SaveData();

        private List<GameDataSaver> _gameSavers = new List<GameDataSaver>();

        public event Action<Timestamp?> OnPreLoad;
        public event Action<Timestamp?> OnLoadFinished ;
        public event Action<Timestamp?> OnPreSave;
        public event Action OnSaveFinished;

        public Timestamp? LastSaveTimestamp
        {
            get => _saveData?.SaveTimestamp;
        }

        private void Start()
        {
            LoadGameData();
        }

        private void OnDestroy()
        {
            foreach(var gameSaver in _gameSavers)
            {
                gameSaver.OnSaveDataChanged -= HandleSaveDataChanged;
            }
        }

        private void LoadGameData()
        {
            _gameSavers.Clear();

            var gameSavers = FindObjectsOfType(typeof(GameDataSaver));
            foreach(var gameSaverObject in gameSavers)
            {
                var gameSaver = gameSaverObject as GameDataSaver;
                if (_gameSavers.Contains(gameSaver))
                {
                    continue;
                }

                _gameSavers.Add(gameSaver);
                gameSaver.OnSaveDataChanged += HandleSaveDataChanged;
            }

            _gameSavers.Sort((g1, g2) => g1.Priority.CompareTo(g2.Priority));

            LoadDataFile();
        }

        private void HandleSaveDataChanged(SaveDataHandler dataHandler)
        {
            SaveDataFile();
        }

        private void LoadDataFile()
        {
            OnPreLoad?.Invoke(LastSaveTimestamp);

            if (!TryLoadDataFile(out _saveData))
            {
                _saveData = new SaveData()
                {
                    SaveTimestamp = Timestamp.Now
                };
            }

            foreach (var gameSaver in _gameSavers)
            {
                var targetId = gameSaver.TargetId;
                var saveHandler = _saveData[targetId];

                gameSaver.LoadSaveData(saveHandler, LastSaveTimestamp);
            }

            LoadFinished(LastSaveTimestamp);
        }

        private void LoadFinished(Timestamp? lastSaveTimestamp)
        {
            foreach (var gameSaver in _gameSavers)
            {
                gameSaver.LoadFinished(lastSaveTimestamp);
            }

            OnLoadFinished?.Invoke(lastSaveTimestamp);
        }

        private TryResult TryLoadDataFile(out SaveData saveData)
        {
            var filePath = GetSaveFilePath(FILE_NAME);
            if (!File.Exists(filePath))
            {
                saveData = null;
                return TryResult.Fail;
            }

            var json = File.ReadAllText(filePath, Encoding.UTF8);
            saveData = JsonUtility.FromJson<SaveData>(json);
            return TryResult.Successfully;
        }

        private void SaveDataFile()
        {
            OnPreSave?.Invoke(LastSaveTimestamp);

            TrySaveDataFile(_saveData);

            SaveFinished();
        }

        private TryResult TrySaveDataFile(SaveData saveData)
        {
            var filePath = GetSaveFilePath(FILE_NAME);

            foreach(var gameSaver in _gameSavers)
            {
                var dataHandler = gameSaver.GetSaveDataHandler();
                _saveData.SetDataHandler(dataHandler);
            }

            _saveData.SaveTimestamp = Timestamp.Now;

            var json = JsonUtility.ToJson(saveData, true);
            using (var sw = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                sw.WriteLine(json);
            }

            return TryResult.Successfully;
        }

        private void SaveFinished()
        {
            OnSaveFinished?.Invoke();
        }

        private static string GetSaveFilePath(string fileName)
        {
            return Path.Combine(UnityEngine.Application.persistentDataPath, fileName);
        }

#if UNITY_EDITOR
        [MenuItem("[RMG] Tools/Reset save data")]
#endif
        public static void ResetSaveData()
        {
            var filePath = GetSaveFilePath(FILE_NAME);
            if (!File.Exists(filePath))
            {
                return;
            }

            File.Delete(filePath);
        }
    }
}
