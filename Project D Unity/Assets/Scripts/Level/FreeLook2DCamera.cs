using Cinemachine;
using CoreSystems.InputSystem;
using UnityEngine;

namespace Level
{
    /// <summary>
    /// Sets the behaviour of the camera to be able to look around the level
    /// </summary>
    public class FreeLook2DCamera : MonoBehaviour
    {
        public Transform player;
        public Transform freeLookTarget;
        
        private const float _speed = 5;
        
        /// <summary>
        /// The base size of the camera when it is not on free look
        /// </summary>
        private const float _baseOrthographicSize = 10f;
        
        /// <summary>
        /// The camera size when it is on free look mode
        /// </summary>
        private const float _freeLookOrthographicSize = 12f;
        
        private CinemachineVirtualCamera _virtualCamera;
        
        /// <summary>
        /// How much the camera has moved during free look
        /// </summary>
        private Vector2 _freeLookOffset;

        
        #region Unity functions    
        private void Start()
        {
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        }

        private void OnEnable()
        {
            FreeLookInput.StartFreeLook += OnFreeLookStart;
            FreeLookInput.StopFreeLook += OnFreeLookStop;
            FreeLookInput.FreeLookCameraMove += OnFreeLookCameraMove;
        }

        private void OnDisable()
        {
            FreeLookInput.StartFreeLook -= OnFreeLookStart; 
            FreeLookInput.StopFreeLook -= OnFreeLookStop;
            FreeLookInput.FreeLookCameraMove -= OnFreeLookCameraMove; 
        }

        #endregion    
        
        #region Private functions

        /// <summary>
        /// Logic to be executed when the free look start event is invoked
        /// </summary>
        private void OnFreeLookStart()
        {
            _virtualCamera.Follow = freeLookTarget;
            _virtualCamera.m_Lens.OrthographicSize = _freeLookOrthographicSize;
        }

        /// <summary>
        /// Logic to be executed when the free look stop event is invoked
        /// </summary>
        private void OnFreeLookStop()
        {
            _virtualCamera.Follow = player;
            _virtualCamera.m_Lens.OrthographicSize = _baseOrthographicSize;
            freeLookTarget.position = player.position;
        }

        /// <summary>
        /// Handles the camera movement based on the input received
        /// </summary>
        /// <param name="offset">The distance and direction to move the camera</param>
        private void OnFreeLookCameraMove(Vector2 offset)
        {
            freeLookTarget.position = offset  * _speed;
        }
        
        #endregion
    }
}