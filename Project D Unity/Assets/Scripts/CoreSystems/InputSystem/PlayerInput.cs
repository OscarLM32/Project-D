using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

namespace CoreSystems.InputSystem
{
    /// <summary>
    /// Manages the player input to control the character
    /// </summary>
    public class PlayerInput : MonoBehaviour
    {
        public Action<Vector2> InputReceived; 
        
        /// <summary>
        /// Minimum distance you have to swipe
        /// </summary>
        private const float _minSwipeDistance = 10f;
        
        private Vector2 _travelledDistance;
        private Vector2 _touchPoint;
        
        
        private void Update()
        {
            //If there are no touches or more than 1 do nothing
            if (Input.touches.Length == 0 || Input.touches.Length > 1)
            {
                return;
            }

            var touch = Input.touches[0];
            HandleTouch(touch);

            CheckSwipeDone();
        }

        /// <summary>
        /// Depending on the phase of the touch a logic will be conducted
        /// </summary>
        /// <param name="touch">The touch</param>
        private void HandleTouch(Touch touch)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _touchPoint = touch.position;
                    break;
                case TouchPhase.Moved:
                    OnTouchMoved(touch);
                    break;
                case TouchPhase.Ended:
                    _touchPoint = Vector2.zero;
                    _travelledDistance = Vector2.zero;
                    break;
            }
        }

        /// <summary>
        /// Logic to be executed when the player touch is moved
        /// </summary>
        /// <param name="touch">The touch</param>
        private void OnTouchMoved(Touch touch)
        {
            _travelledDistance = CalculateTravelledDistance(touch);
            if (CheckSwipeDone())
            {
                HandleSwipe();
            }
        }

        /// <summary>
        /// Calculates how long has the touch moved from the original point
        /// </summary>
        /// <param name="touch">The point</param>
        /// <returns>The distance travelled expressed as a Vector2</returns>
        private Vector2 CalculateTravelledDistance(Touch touch)
        {
            var distance = Vector2.Distance(_touchPoint, touch.position);
            var pos = touch.position - _touchPoint;
            var angle = Math.Atan2(pos.x, pos.y);

            return new Vector2(distance * (float)Math.Sin(angle), distance * (float)Math.Cos(angle));
        }

        /// <summary>
        /// Checks if the current touch has travelled a long enough distance to be considered a swipe
        /// </summary>
        /// <returns></returns>
        private bool CheckSwipeDone()
        {
            return _travelledDistance.x is >= _minSwipeDistance or <= -_minSwipeDistance ||
                   _travelledDistance.y is >= _minSwipeDistance or <= -_minSwipeDistance;
        }

        /// <summary>
        /// Handles the current swipe direction and invoke the InputReceived action
        /// </summary>
        private void HandleSwipe()
        {
            if (_travelledDistance.x >= _minSwipeDistance)
            {
                InputReceived?.Invoke(Vector2.right);
            }
            else if (_travelledDistance.x <= -_minSwipeDistance)
            {
                InputReceived?.Invoke(Vector2.left);
            }
            else if (_travelledDistance.y >= _minSwipeDistance)
            {
                InputReceived?.Invoke(Vector2.up);
            }
            else
            {
                InputReceived.Invoke(Vector2.down);
            }
            _travelledDistance = Vector2.zero;
        }
    }
}