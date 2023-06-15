using UnityEngine;

namespace Level.DifficultySettings
{
    /// <summary>
    /// Scriptable object containing the difficulty settings of the spikes
    /// </summary> 
    [CreateAssetMenu(fileName = "SpikesDifficultySettings", menuName = "DifficultySetting/Spikes")] 
    public class SOSpikesDifficultySettings : ScriptableObject
    {
        public float[] activationTime;
    }
}