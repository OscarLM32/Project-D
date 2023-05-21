using System;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Class for testing the player movement
    /// </summary>
    public class MovementTesting : MonoBehaviour
    {
        public Action<Vector2> InputReceived; 
    
    #if UNITY_EDITOR    
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                InputReceived?.Invoke(Vector2.up);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                InputReceived?.Invoke(Vector2.down); 
            }        
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                InputReceived?.Invoke(Vector2.left); 
            }        
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                InputReceived?.Invoke(Vector2.right); 
            }
        }
    #endif    
    } 
}

