using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MainMenu;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CoreSystems.SaveLoadSystem
{
    /// <summary>
    /// Handles the logic for saving, loading and managing data
    /// </summary>
    public class SaveLoadManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static SaveLoadManager Instance { get; private set; }

        /// <summary>
        /// Path to where the data is stored
        /// </summary>
        private string _savePath;
        
        /// <summary>
        /// Formatter to serialize and deserialize the data
        /// </summary>
        private BinaryFormatter _formatter;

        [SerializeField]private GameData _gameData;
        [SerializeField]private TemporalGameData _tempGameData;

        /// <summary>
        /// Get the current scene name
        /// </summary>
        private string currentSceneName => SceneManager.GetActiveScene().name;

        #region Unity functions

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            Configure();
        }

        #endregion
        
        #region Public functions
        
        
        /// <summary>
        /// Saves the game data
        /// </summary>
        public void Save()
        {
            FileStream stream = new FileStream(_savePath, FileMode.OpenOrCreate);
        
            AddTemporalDataToSavedData();
            CheckCurrentLevelIsMaxLevelReached();
            
            _formatter.Serialize(stream, _gameData);
            stream.Close();
            
            AppLogger.Log(DebugFlag.SAVE_LOAD_SYSTEM, "Data saved");
        }

        /// <summary>
        /// Get the max level reached value that has been saved
        /// </summary>
        /// <returns></returns>
        public int GetMaxLevelReached()
        {
            return _gameData.maxLevelReached;
        }
        
        /// <summary>
        /// Get the level bitmap of the current level
        /// </summary>
        /// <returns>A bool array</returns>
        public bool[] GetCurrentLevelClassifiedDocumentsBitmap()
        {
            if (!_gameData.levelClassifiedDocumentsBitmap.ContainsKey(currentSceneName))
            {
                _gameData.levelClassifiedDocumentsBitmap.Add(currentSceneName, new []{true,true,true});
                AppLogger.Log(DebugFlag.SAVE_LOAD_SYSTEM, "There is no bitmap data saved for this level");
            }
            return _gameData.levelClassifiedDocumentsBitmap[currentSceneName];
        }

        /// <summary>
        /// get the level bitmap of a specified level
        /// </summary>
        /// <param name="level">The level</param>
        /// <returns>A bool array</returns>
        public bool[] GetClassifiedDocumentsBitmap(string level)
        {
            if (!_gameData.levelClassifiedDocumentsBitmap.ContainsKey(level))
            {
                AppLogger.LogError(DebugFlag.SAVE_LOAD_SYSTEM, "There is no saved data related to the level: " +level);
                return null;
            }
            return _gameData.levelClassifiedDocumentsBitmap[level];
        }

        /// <summary>
        /// Add picked information to the temporal data
        /// </summary>
        /// <param name="value">Amount</param>
        public void AddPickedInformation(int value)
        {
            _tempGameData.information += value;
        }

        /// <summary>
        /// Add picked classified documents to the temporal data and set the temporal bitmap properly
        /// </summary>
        /// <param name="pos">The position in the level (first, second or third)</param>
        public void AddPickedClassifiedDocument(int pos)
        {
            //This currency can only be obtained 1 by 1
            _tempGameData.classifiedDocuments++;
            //Place the classified document's value to false
            _tempGameData.classifiedDocumentsBitmap[pos] = false;
        }

        /// <summary>
        /// Get the information stored in the game data
        /// </summary>
        /// <returns>The information value</returns>
        public int GetInformation()
        {
            return _gameData.information;
        }

        /// <summary>
        /// Get the classified documents stored in the game data
        /// </summary>
        /// <returns>The classified documents value</returns>
        public int GetClassifiedDocuments()
        {
            return _gameData.classifiedDocuments;
        }

        /// <summary>
        /// Sets the data after buying an upgrade from the game store
        /// </summary>
        /// <param name="type">The upgrade type</param>
        /// <param name="value">The cost</param>
        public void BuyUpgrade(UpgradeType type, int value)
        {
            if (!_gameData.upgradesBought.ContainsKey(type))
            {
                _gameData.upgradesBought.Add(type, 1); 
            }
            else
            {
                _gameData.upgradesBought[type]++;
            }

            _gameData.information -= value;
            //TODO: solve this huge dependency
            CurrencyPresenter.UpdateInformation?.Invoke();

            Save();
        }

        /// <summary>
        /// Get how many times an upgrade was bought
        /// </summary>
        /// <param name="type">The upgrade type</param>
        /// <returns>The amount of time it was bought</returns>
        public int GetTimesUpgradeWasBought(UpgradeType type)
        {
            if (!_gameData.upgradesBought.ContainsKey(type))
            {
                return 0;
            }
            return _gameData.upgradesBought[type];
        }

        #endregion

        #region Private functions

        /// <summary>
        /// Configures the instance
        /// </summary>
        private void Configure()
        {
            Instance = this;
            _savePath = Application.persistentDataPath + "/gameData.save";
            _formatter = new BinaryFormatter();
            Load();
            //DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Load the data stored
        /// </summary>
        private void Load()
        {
            if (File.Exists(_savePath))
            {
                FileStream stream = new FileStream(_savePath, FileMode.Open);

                if (stream.Length == 0)
                {
                    AppLogger.LogError(DebugFlag.SAVE_LOAD_SYSTEM, "The stream you are trying to read is empty");
                    return;
                }
                
                _gameData = _formatter.Deserialize(stream) as GameData;
                stream.Close();
                
                _tempGameData = new TemporalGameData();
                if (!_gameData.levelClassifiedDocumentsBitmap.ContainsKey(currentSceneName)) return;
                _tempGameData.classifiedDocumentsBitmap = _gameData.levelClassifiedDocumentsBitmap[currentSceneName];
            }
            else
            {
                _gameData = new GameData();
                _tempGameData = new TemporalGameData(); 
                Debug.Log("The save file was not found in path: " + _savePath);
            }
        }

        /// <summary>
        /// Adds the temporal objects obtained in a level to the permanent data
        /// </summary>
        private void AddTemporalDataToSavedData()
        {
            _gameData.information += _tempGameData.information;
            _gameData.classifiedDocuments += _tempGameData.classifiedDocuments;
            _gameData.levelClassifiedDocumentsBitmap[currentSceneName] = _tempGameData.classifiedDocumentsBitmap;
        }

        /// <summary>
        /// Checks if the current level is the maximum beaten and updates it in that case
        /// </summary>
        private void CheckCurrentLevelIsMaxLevelReached()
        {
            int lvl = CurrentLevelToLevelInt();
            if (lvl == _gameData.maxLevelReached)
            {
                _gameData.maxLevelReached = lvl+1;
                AppLogger.Log(DebugFlag.SAVE_LOAD_SYSTEM, "New max level updated");
            }
        }

        /// <summary>
        /// Returns the current level expressed as an integer
        /// </summary>
        /// <returns>The integer representing the level</returns>
        private int CurrentLevelToLevelInt()
        {
            //All the "level" scenes are named "Level" followed by the number with no space.
            string sNum = currentSceneName.Substring(5); //It can be also written as [5..]
            int lvl = 0;
            //TODO: this is caused because of a huge dependency between save load manager and presenters
            try
            {
                lvl = int.Parse(sNum);  
            }
            catch (Exception e)
            {
                AppLogger.Log(DebugFlag.SAVE_LOAD_SYSTEM, e.ToString());
                return 0;
            }

            return lvl;
        }

        #endregion

        /// <summary>
        /// The representation of the permanent game data
        /// </summary>
        [Serializable]
        private class GameData
        {
            public int maxLevelReached = 1;
            public Dictionary<UpgradeType, int> upgradesBought = new Dictionary<UpgradeType, int>();

            public int information;
            public int classifiedDocuments;
            public Dictionary<string, bool[]> levelClassifiedDocumentsBitmap = new Dictionary<string, bool[]>();
        }

        /// <summary>
        /// Representation of temporal game data
        /// </summary>
        [Serializable]
        private class TemporalGameData
        {
            public int information;
            public int classifiedDocuments;
            public bool[] classifiedDocumentsBitmap = new []{true, true, true};
        }
    }

    /// <summary>
    /// A list of the upgrades types
    /// </summary>
    public enum UpgradeType
    {
        LIFE,
        TIME
    }
}
