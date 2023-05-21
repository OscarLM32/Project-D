using System;
using UnityEngine;

namespace CoreSystems.InputSystem
{
    /// <summary>
    /// Handles the logic behind controlling the free look camera
    /// </summary>
    public class FreeLookInput : MonoBehaviour
    {
        public static event Action StartFreeLook;
        public static event Action StopFreeLook;
        /// <summary>
        /// Action to be called when an input to move the camera is received
        /// </summary>
        public static event Action<Vector2> FreeLookCameraMove;

        /// <summary>
        /// The minimum distance the fingers must travel for the gesture to be accepted 
        /// </summary>
        private const int _minZoomGestureDistanceDifference = 25;
        private float _startingGestureDistance;
        private bool _freeLookStarted = false; 
        
        private Vector2 _offset = Vector2.zero;
        private Touch[] _touches;

        private void Update()
        {
            //If the touches are more than 2 or 0 do nothing
            _touches = Input.touches;
            if (_touches.Length == 2)
            {
                CheckGestureStart();
                CheckFreeLookStart();
                CheckFreeLookStop();
                CheckGestureStop();   
            }

            if (_touches.Length == 1 && _freeLookStarted)
            {
                HandleMovement();
            }

        }

        /// <summary>
        /// Checks if the user is starting the start or stop free look gesture checking if one of the 2 touches
        /// just started and calculating the distance between them
        /// </summary>
        private void CheckGestureStart()
        {
            //If none of the two touches is beginning do nothing
            if (_touches[0].phase != TouchPhase.Began && _touches[1].phase != TouchPhase.Began) return;

            _startingGestureDistance = Vector2.Distance(_touches[0].position, _touches[1].position);
        }

        /// <summary>
        /// Checks if the user is stopping the gesture by lifting one of the 2 fingers
        /// </summary>
        private void CheckGestureStop()
        {
            if (_touches[0].phase == TouchPhase.Ended || _touches[1].phase == TouchPhase.Ended)
            {
                Debug.Log("GestureStopped");
                _startingGestureDistance = 0;
            }
        }

        /// <summary>
        /// Checks if the gesture done by the touches is the gesture to start the free look mode checking
        /// if the distance between the two fingers has decreased by at least the minimum gesture distance
        /// </summary>
        private void CheckFreeLookStart()
        {
            if (_freeLookStarted) return; //Do not start the free look multiple times
            
            var distance = Vector2.Distance(_touches[0].position, _touches[1].position);
            if (_startingGestureDistance - distance >= _minZoomGestureDistanceDifference)
            {
                StartFreeLook?.Invoke();
                _freeLookStarted = true;
            }
        }

        /// <summary>
        /// Checks if the gesture done by the touches is the gesture to stop the free look mode checking
        /// if the distance between the two fingers has increased by at least the minimum gesture distance
        /// </summary>
        private void CheckFreeLookStop()
        {
            if (!_freeLookStarted) return;
            
            var distance = Vector2.Distance(_touches[0].position, _touches[1].position);
            if (distance - _startingGestureDistance >= _minZoomGestureDistanceDifference)
            {
                StopFreeLook?.Invoke();
                _freeLookStarted = false;
                _offset = Vector2.zero;
            } 
        }

        /// <summary>
        /// Calculates how much has the finger id has moved from its initial position and sends it through an action
        /// </summary>
        private void HandleMovement()
        {
            var touch = _touches[0];
            _offset -= touch.deltaPosition * Time.deltaTime;
            FreeLookCameraMove?.Invoke(_offset);
        }
    }
}