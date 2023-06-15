using UnityEngine;

namespace Level.DifficultySettings
{
    /// <summary>
    /// Scriptable object containing the difficulty setting for the shooting enemies
    /// </summary> 
    [CreateAssetMenu(fileName = "ShootingEnemyDifficultySettings", menuName = "DifficultySetting/ShootingEnemy")] 
    public class SOShootingEnemyDifficultySettings : ScriptableObject
    {
        public float[] attackRate;
    }
}