using System;
using System.Collections;
using Level;
using Level.DifficultySettings;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Handles the logic behind the spike trap that can kill the player
    /// </summary>
    public class  Spikes : MonoBehaviour
    {
        public SOSpikesDifficultySettings difficultySettings;

        private AudioSource _audioSource;
        private Animator _animator;

        /// <summary>
        /// Name of the animation for spike to come out
        /// </summary>
        private const string SpikesOut = "SpikesOut";
        /// <summary>
        /// Name of the animation for the spikes to prepare to come out
        /// </summary>
        private const string SpikesPrepare = "SpikesPrepare";
        
        private float _trapActivationTime = 1.5f; 

        [SerializeField]private bool _isTrapActivated = false;
        [SerializeField]private bool _trapBeingActivated = false;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _animator = GetComponent<Animator>();

            _animator.enabled = false;
            
            ConfigureDifficulty();
        }
        
        /// <summary>
        /// Handle the logic of the trap. If it is activated it kills the player.
        /// In other case activates it if is not being activated
        /// </summary>
        /// <param name="other">The other gameobject</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.GetComponent<Player.Player>(); 
            if (!player) return;
            
            if (!_isTrapActivated && !_trapBeingActivated)
            {
                StartCoroutine(ActivateTrap());
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            Debug.Log("OnTriggerStay " + Time.time);
            var player = other.transform.GetComponent<Player.Player>(); 
            if(_isTrapActivated)
            {
                Debug.Log("I kill");
                player.Kill();
                _isTrapActivated = false;
            } 
        }

        /// <summary>
        /// Clears the different routines the script has running
        /// </summary>
        private void OnDestroy()
        {
            StopCoroutine(DeactivateTrap()); 
            StopCoroutine(ActivateTrap());
        }

        private void ConfigureDifficulty()
        {
            _trapActivationTime = difficultySettings.activationTime[(int) GameManager.Instance.difficulty];
        }

        /// <summary>
        /// Activates the trap and sets the coroutine to deactivate it
        /// </summary>
        /// <returns>A routine</returns>
        private IEnumerator ActivateTrap()
        {
            Debug.Log("Activating trap " + Time.time);
            _animator.enabled = true;
            _animator.Play(SpikesPrepare); 
            _trapBeingActivated = true;
            
            yield return new WaitForSeconds(_trapActivationTime);

            _audioSource.Play();
            _animator.Play(SpikesOut);
            _isTrapActivated = true;
            _trapBeingActivated = false;

            StartCoroutine(DeactivateTrap());
        }
        
            
        /// <summary>
        /// Deactivates the trap
        /// </summary>
        /// <returns>A routine</returns>
        private IEnumerator DeactivateTrap()
        {
            Debug.Log("DeactivatingTrap " + Time.time);
            while (!_animator.GetCurrentAnimatorStateInfo(0).IsName(SpikesOut))
            {
                yield return null;
            }
            
            while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                yield return null;
            }

            Debug.Log("Trap deactivated");  
            _isTrapActivated = false;
        }
    }
}