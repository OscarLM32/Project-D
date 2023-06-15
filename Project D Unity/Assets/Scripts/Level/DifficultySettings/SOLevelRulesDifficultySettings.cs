using UnityEngine;

namespace Level.DifficultySettings
{
    /// <summary>
    /// Scriptable object containing the difficulty information of the level
    /// </summary>
    [CreateAssetMenu(fileName = "LevelRulesDifficultySettings", menuName = "DifficultySetting/LevelRules")]
    public class SOLevelRulesDifficultySettings : ScriptableObject
    {
        public int[] extraTime;
        public int[] extraLives;
    }
}