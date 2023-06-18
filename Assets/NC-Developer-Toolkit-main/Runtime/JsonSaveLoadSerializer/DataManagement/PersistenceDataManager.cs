using System.Collections.Generic;
using System.Linq;
using JsonSaveLoadSerializer.Dependencies;
using UnityEngine;

namespace JsonSaveLoadSerializer.DataManagement
{
    public class PersistenceDataManager : BaseMonoSingleton<PersistenceDataManager>
    {
        private GameData _gameData;
        private List<IDataPersistence> _dataPersistenceObjects;
        private FileDataHandler _dataHandler;
        
        [SerializeField, Tooltip("File name to save inside application persistent data folder")] private string fileName;
        [SerializeField, Tooltip("Use encryption to save and load data")] private bool useEncryption;

        protected override void Start()
        {
            //TODO: SAVE LOAD PATH DOES NOT WORK FOR WEBGL!!
            _dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
            _dataPersistenceObjects = FetchAllPersistenceObjects();
            base.Start();
        }
        private static List<IDataPersistence> FetchAllPersistenceObjects()
        {
            return FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>().ToList();
        }
        public void CreateData()
        {
            _gameData = new GameData();
        }

        public void LoadData()
        {
            _gameData = _dataHandler.Load();
            
            if (_gameData == null)
            {
                Debug.LogWarning("||| Safeguard Warning! No data was found. Creating new data object from default");
                CreateData();
            }
            foreach (var dataObject in _dataPersistenceObjects)
            {
                dataObject.LoadData(_gameData);
            }
        }
        public void SaveData()
        {
            foreach (var dataObject in _dataPersistenceObjects)
            {
                dataObject.SaveData(ref _gameData);
            }
            
            _dataHandler.Save(_gameData);
        }
        private void OnApplicationQuit() => SaveData();
    }
}
