using System;
using CoreSystems.AudioSystem;
using UnityEngine;
using AudioType = CoreSystems.AudioSystem.AudioType;

namespace Level
{
    /// <summary>
    /// Handles the logic of the object positioned at the end of each level
    /// </summary>
    public class LevelEnd : MonoBehaviour
    {
        public AudioTrack[] tracks;

        private void Start()
        {
            AudioController.instance.AddTracks(tracks, gameObject);
        }

        /// <summary>
        /// Handles the logic for leaving the level
        /// </summary>
        /// <param name="other">The other game object</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            Player.Player player = other.GetComponent<Player.Player>();
            
            if (player != null)
            {
                AudioController.instance.PlayAudio(AudioType.SFX_VICTORY, gameObject, delay: 0.1f);
                GameEvents.LevelFinished?.Invoke();
            }
        }
    }
}