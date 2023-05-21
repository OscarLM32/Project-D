using System.Collections;
using UnityEngine;

namespace CoreSystems.InputSystem
{
    /// <summary>
    /// Handles the enabling or disabling of the different inputs
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance;

        public FreeLookInput freeLookInput;
        public PlayerInput playerInput;

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        /// <summary>
        /// Disables all inputs
        /// </summary>
        public void DisableInputs()
        {
            freeLookInput.enabled = false;
            playerInput.enabled = false;
        }

        /// <summary>
        /// Enables all inputs
        /// </summary>
        public void EnableInputs()
        {
            freeLookInput.enabled = true;
            playerInput.enabled = true;
        }

        private void OnEnable()
        {
            FreeLookInput.StartFreeLook += OnFreeLookStart;
            FreeLookInput.StopFreeLook += OnFreeLookStop;
        }

        private void OnDisable()
        {
            FreeLookInput.StartFreeLook -= OnFreeLookStart;
            FreeLookInput.StopFreeLook -= OnFreeLookStop;
        }

        /// <summary>
        /// Logic to be executed when the free look mode starts: disable the player input
        /// </summary>
        private void OnFreeLookStart()
        {
            playerInput.enabled = false;
        }

        /// <summary>
        /// Logic to be executed when the free look mode stops
        /// </summary>
        private void OnFreeLookStop()
        {
            StartCoroutine(OnFreeLookStopCoroutine());
        }

        /// <summary>
        /// Coroutine for the behaviour to be executed when the free look mode is stopped: enable player
        /// input after a short period of time
        /// </summary>
        /// <returns>A coroutine</returns>
        private IEnumerator OnFreeLookStopCoroutine()
        {
            yield return new WaitForSeconds(0.2f);
            playerInput.enabled = true; 
        }
    }
}