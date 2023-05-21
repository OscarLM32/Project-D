using CoreSystems.AudioSystem;
using CoreSystems.InputSystem;
using Items;
using Level;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Main class of the player behaviour controlling the rest of the player scripts
    /// </summary>
    public class Player : MonoBehaviour
    {
        public bool debug;

        public AudioTrack[] tracks;
        
        private PlayerMovement _movement;
        private PlayerInput _input;
        
        private Collider2D _col;
        
    #region Unity functions
        
        private void Awake()
        {
            _movement = GetComponent<PlayerMovement>();
            _input = GetComponent<PlayerInput>();

            _col = GetComponent<Collider2D>();
        }

        private void Start()
        {
            AudioController.instance.AddTracks(tracks, gameObject); 
        }

        private void OnEnable()
        {
            _input.InputReceived += OnInputReceived;
            GameEvents.PlayerRespawn += OnPlayerRespawn;
        }
            
        private void OnDisable()
        {
            _input.InputReceived -= OnInputReceived;
            GameEvents.PlayerRespawn -= OnPlayerRespawn; 
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var pickable = other.gameObject.GetComponent<IPickable>();
            pickable?.PickUp();
        }

        #endregion
    
        #region Public functions

        /// <summary>
        /// Logic to be executed when the player is killed
        /// </summary>
        public void Kill()
        {
            //Disable interaction
            _input.enabled = false;
            _col.enabled = false;
            _movement.StopMovement();
            
            //Notify the event
            GameEvents.PlayerDeath?.Invoke();
            
            //TODO: Finish Player "kill" logic
            //Play death audio and animation
            //AudioController.instance.PlayAudio();
            //_animator.Play("PlayerDeath")
        }
    
        #endregion
            
        #region Private functions    
    
        /// <summary>
        /// When the player input signals that an input has been received, it sends it to the player movement script
        /// </summary>
        /// <param name="direction">Direction of the input</param>
        private void OnInputReceived(Vector2 direction)
        {
            _movement.AddMove(direction);
        }

        /// <summary>
        /// Logic to handle whenever the player is spawned
        /// </summary>
        private void OnPlayerRespawn()
        {
            //TODO: finish player respawn logic
            //AudioController.instance.PlayAudio();
            //_animator.Play("PlayerIdle")
            _col.enabled = true;
        }
        
        private void Log(string msg)
        {
            if (!debug) return;
            Debug.Log("[Player]: " + msg);
        }

        private void LogWarning(string msg)
        {
            if (!debug) return;
            Debug.LogWarning("[Player]: " + msg); 
        }

        #endregion
    }
}