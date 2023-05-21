using CoreSystems.AudioSystem;
using UnityEngine;
using AudioType = CoreSystems.AudioSystem.AudioType;
using Vector2 = UnityEngine.Vector2;

namespace Player
{
    /// <summary>
    /// Handles the movement of the player
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        /// <summary>
        /// Different movement states of the player
        /// </summary>
        private enum MovementState
        {
            MOVING,
            IDLE
        }
        
        private MovementState _currentMovementState = MovementState.IDLE;

        public bool debug;
        
        [SerializeField]private LayerMask _groundLayer;

        /// <summary>
        /// Where the input signals the player wants to be
        /// </summary>
        private Vector2 _desiredPosition;
        
        /// <summary>
        /// Where the player currently is
        /// </summary>
        private Vector2 _initialPosition;
        
        private float _distanceToDesiredPosition;
        /// <summary>
        /// The percentage of the current distance traveled from the initial position to the desired one
        /// </summary>
        private float _totalDistanceTravelled;
        
        /// <summary>
        /// The move being performed
        /// </summary>
        private Vector2 _currentMovement = Vector2.zero;
        /// <summary>
        /// The next stored move
        /// </summary>
        private Vector2 _nextMovement = Vector2.zero;
        //private Vector2 _lastMovement = Vector2.zero;
        
        private readonly Vector2 _zero = Vector2.zero; //Abbreviation of Vector2.zero 
        private const float _speed = 40f;
        
        /// <summary>
        /// Visual extent of the character
        /// </summary>
        private const float _characterExtent = 0.5f;
        
        #region Unity Functions    

        private void Update()
        {
            if (_currentMovementState == MovementState.MOVING)
            {
                Move();
            }
        }

        #endregion
    
        #region Public functions    
        
        /// <summary>
        /// Adds a move to the current move if none is being performed. Otherwise, stores it as next movement
        /// if it is different to the current one
        /// </summary>
        /// <param name="direction">The direction of the movement</param>
        public void AddMove(Vector2 direction)
        {
            if (_currentMovement == _zero)
            {
                _currentMovement = direction;
                SetUpMovement();
                return; //No need to follow with the logic 
            }

            //There is no real need to have double movement sequences
            if (_currentMovement != direction)
            {
                _nextMovement = direction;
            }
        }

        /// <summary>
        /// Stops all movements, current one and next one
        /// </summary>
        public void StopMovement()
        {
            DisposeCurrentMovement();
            _nextMovement = _zero;
        }
        
        #endregion
        
        #region Private Functions
        /// <summary>
        /// Starts the movement stored after finishing the current movement
        /// </summary>
        private void StartNextMovement()
        {
            if (_nextMovement == _zero) return;

            _currentMovement = _nextMovement;
            _nextMovement = _zero;
            
            SetUpMovement();
        }     
        
        /// <summary>
        /// Sets up the movement the player has signaled
        /// </summary>
        private void SetUpMovement()
        {
            //TODO: check why this is not working
            //RaycastHit2D ray = Physics2D.Raycast(transform.position, _currentMovement, layerMask: _groundLayer); 
            RaycastHit2D ray = Physics2D.Raycast(transform.position, _currentMovement,Mathf.Infinity, layerMask: _groundLayer);
            
            if(!ray.transform)
            { 
               DisposeCurrentMovement();
               LogWarning("No wall/ground was found for the player to dash to");
               return;
            }

            var position = transform.position;
            Vector2 desiredPos =  (Vector2)position + ray.distance * _currentMovement;
            _desiredPosition = desiredPos - _currentMovement * _characterExtent;

            //If the movement is going to lead the player to the same location return
            if (_desiredPosition == (Vector2)position)
            {
                DisposeCurrentMovement();
                return;
            }
            
            _initialPosition = position;
            _distanceToDesiredPosition = Vector2.Distance(_initialPosition, _desiredPosition);

            _currentMovementState = MovementState.MOVING;
            
            Log("New movement to " + _desiredPosition + " has been set up");
        }
        
        /// <summary>
        /// Moves the character
        /// </summary>
        private void Move()
        {
            transform.position = Vector2.Lerp(_initialPosition, _desiredPosition, _totalDistanceTravelled);
            _totalDistanceTravelled += _speed * Time.deltaTime / _distanceToDesiredPosition;

            if ((Vector2)transform.position == _desiredPosition)
            {
                FinishMovement();
                //Log("The movement to: "+_desiredPosition+" has terminated");
            }
        }

        /// <summary>
        /// Disposes of the set up of the current movement
        /// </summary>
        //If the movement being set up does not meet proper conditions
        private void DisposeCurrentMovement()
        {
            _currentMovementState = MovementState.IDLE;
            _currentMovement = _zero;
            _totalDistanceTravelled = 0;
        }

        /// <summary>
        /// Completes the current movement
        /// </summary>
        //The current movement has been completed
        private void FinishMovement()
        {
            DisposeCurrentMovement();
            
            AudioController.instance.PlayAudio(AudioType.SFX_MOVEMENT_LANDING, gameObject); 

            StartNextMovement(); 
        }
        
        private void Log(string msg)
        {
            if (!debug) return;
            Debug.Log("[PlayerMovement]: " + msg);
        }

        private void LogWarning(string msg)
        {
            if (!debug) return;
            Debug.LogWarning("[PlayerMovement]: " + msg); 
        } 
        
        #endregion    
    }
}