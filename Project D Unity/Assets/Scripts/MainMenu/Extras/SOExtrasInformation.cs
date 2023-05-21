using UnityEngine;

namespace MainMenu.Extras
{
    /// <summary>
    /// Scriptable object containing the extra content and values of each extra
    /// </summary>
    [CreateAssetMenu(menuName="Extras/ExtrasInformation", fileName = "ExtrasInformation")]
    public class SOExtrasInformation : ScriptableObject
    {
        public int[] values;

        [TextArea]
        public string[] texts;
    }
}