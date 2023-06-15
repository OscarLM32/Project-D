using System.Collections;
using CoreSystems.AudioSystem;
using CoreSystems.InputSystem;
using CoreSystems.MenuSystem;
using CoreSystems.SaveLoadSystem;
using CoreSystems.SceneSystem;
using Level.DifficultySettings;
using UnityEngine;
using AudioType = CoreSystems.AudioSystem.AudioType;

namespace Level
{
    /// <summary>
    /// Controls the game flow by controlling the other managers in the levels
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public Difficulties difficulty = Difficulties.EASY;
        public SOLevelRulesDifficultySettings difficultySettings;
        
        [SerializeField]private Player.Player _player;
        [SerializeField] private Timer _timer;

        [Space]
        public AudioTrack[] tracks;

        private Vector2 _playerSpawnPoint;
        private Quaternion _playerSpawnRotation;
        [SerializeField]private int _maxPlayerDeaths = 1;
        private int _playerDeaths;

        private const int _extraTimeFactor = 10; //The amount of extra time each upgrades gives the player
        
        private const float _timerWaitTimeBeforeStart = 2f;

        public Vector2 playerSpawnPoint
        {
            set => _playerSpawnPoint = value;
        }

        public Quaternion playerSpawnRotation
        {
            set => _playerSpawnRotation = value;
        }

        #region Unity functions    
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            Configure();
        }

        private void OnEnable()
        {
            GameEvents.LevelFinished += FinishLevel;
            GameEvents.PlayerDeath += OnPlayerDeath;
            GameEvents.TimeRanOut += ExitLevel;
        }

        private void OnDisable()
        {
            GameEvents.LevelFinished -= FinishLevel; 
            GameEvents.PlayerDeath -= OnPlayerDeath; 
            GameEvents.TimeRanOut -= ExitLevel; 
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
            AudioController.instance.PlayAudio(AudioType.ST_NAUTILUS, gameObject, 1, 0.5f);

            _playerSpawnPoint = _player.gameObject.transform.position;
            
            ApplyDifficultySettings();
        }

        private int CalculateExtraTime()
        {
            var extraTime = SaveLoadManager.Instance.GetTimesUpgradeWasBought(UpgradeType.TIME) * _extraTimeFactor;
            return extraTime;
        }

        private void ApplyDifficultySettings()
        {
            var extraTime = CalculateExtraTime();
            var extraLives = SaveLoadManager.Instance.GetTimesUpgradeWasBought(UpgradeType.LIFE);

            extraTime += difficultySettings.extraTime[(int)difficulty];
            extraLives += difficultySettings.extraLives[(int)difficulty];
            
            _timer.SetUpTimer(extraTime, _timerWaitTimeBeforeStart);
            _maxPlayerDeaths += extraLives >= 0 ? extraLives : 0;
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
            yield return new WaitForSeconds(1f);
            
            _player.gameObject.transform.position = _playerSpawnPoint;
            GameEvents.PlayerRespawn?.Invoke(_playerSpawnRotation);
            InputManager.Instance.EnableInputs();
        }
        
        #endregion    
    }
}