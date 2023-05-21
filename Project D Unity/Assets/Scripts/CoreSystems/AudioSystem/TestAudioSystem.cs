using UnityEngine;

namespace CoreSystems.AudioSystem
{
    /// <summary>
    /// Testing class for AudioController
    /// </summary>
    public class TestAudioSystem : MonoBehaviour
    {
        public AudioController audioController;
        
    #region Unity functions 
    #if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                audioController.PlayAudio(AudioType.ST_NAUTILUS, gameObject);
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                audioController.StopAudio(AudioType.ST_NAUTILUS, gameObject);
            } 
            if (Input.GetKeyDown(KeyCode.B))
            {
                audioController.RestartAudio(AudioType.ST_NAUTILUS, gameObject);
            } 
            
            if (Input.GetKeyDown(KeyCode.Y))
            {
                audioController.PlayAudio(AudioType.SFX_MOVEMENT_LANDING, gameObject);
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                audioController.StopAudio(AudioType.SFX_MOVEMENT_LANDING, gameObject);
            } 
            if (Input.GetKeyDown(KeyCode.N))
            {
                audioController.RestartAudio(AudioType.SFX_MOVEMENT_LANDING, gameObject);
            }  
        }
        
    #endif    
    #endregion    
    }
}