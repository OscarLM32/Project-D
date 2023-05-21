using System.Collections;
using CoreSystems.AudioSystem;
using CoreSystems.InputSystem;
using CoreSystems.MenuSystem;
using CoreSystems.SaveLoadSystem;
using CoreSystems.SceneSystem;
using UnityEngine;
using AudioType = CoreSystems.AudioSystem.AudioType;

namespace Level
{
    /// <summary>
    /// Controls the game flow by controlling the other managers in the levels
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        
        [SerializeField]private Player.Player _player;

        [Space]
        public AudioTrack[] tracks;

        private Vector2 _playerSpawnPoint;
        [SerializeField]private int _maxPlayerDeaths = 1;
        private int _playerDeaths = 0;

        public Vector2 playerSpawnPoint
        {
            set => _playerSpawnPoint = value;
        }

        #region Unity functions    
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        private void Start()
        {
            Configure();
            //Cannot add this into configure because the instance may not have been created already
            AudioController.instance.PlayAudio(AudioType.ST_NAUTILUS, gameObject, 1, 0.5f);
        }

        private void OnEnable()
        {
            GameEvents.LevelFinished += FinishLevel;
            GameEvents.PlayerDeath += OnPlayerDeath;
        }

        private void OnDisable()
        {
            GameEvents.LevelFinished -= FinishLevel; 
            GameEvents.PlayerDeath -= OnPlayerDeath; 
        }

        #endregion    
        
        #region Private functions 

        /// <summary>
        /// Logic to be executed when the player completes the level
        /// </summary>
        //Invoked when the level is completed
        private void FinishLevel()
        {
            //Disable player controller or player completely
            InputManager.Instance.DisableInputs(); 
            SaveLoadManager.Instance.Save();
            AudioController.instance.StopAudio(AudioType.ST_NAUTILUS, gameObject, 0.1f);
            SceneController.instance.Load(SceneType.MAIN_MENU, loadingPage: PageType.LEVEL_EXIT); 
        }
    
        /// <summary>
        /// Logic to be executed when the player leaves the level without completing it
        /// </summary>
        //Invoked when the level is failed
        private void ExitLevel()
        {
            InputManager.Instance.DisableInputs();
            //Do not save the coins obtained
            AudioController.instance.StopAudio(AudioType.ST_NAUTILUS, gameObject, 0.1f);
            SceneController.instance.Load(SceneType.MAIN_MENU, loadingPage: PageType.LEVEL_EXIT);
        }
        
        /// <summary>
        /// Configures an initial instance of the class
        /// </summary>
        private void Configure()
        {
            AudioController.instance.AddTracks(tracks, gameObject); 
            _playerSpawnPoint = _player.gameObject.transform.position;
            _maxPlayerDeaths += SaveLoadManager.Instance.GetTimesUpgradeWasBought(UpgradeType.LIFE);
        }

        /// <summary>
        /// Checks whether the player is completely dead or if it need to respawn
        /// </summary>
        private void OnPlayerDeath()
        {
            InputManager.Instance.DisableInputs();
            _playerDeaths++;
            if (_playerDeaths == _maxPlayerDeaths)
            {
                ExitLevel();
                return;
            }
            StartCoroutine(PlayerRespawn());
        }

        /// <summary>
        /// Logic to be executed when the player is to be respawned
        /// </summary>
        /// <returns>A coroutine</returns>
        private IEnumerator PlayerRespawn()
        {
            yield return new WaitForSeconds(0.5f);
            
            _player.gameObject.transform.position = _playerSpawnPoint;
            GameEvents.PlayerRespawn?.Invoke();
            InputManager.Instance.EnableInputs();
        }
        
        #endregion    
    }
}