using UnityEngine;

namespace MainMenu.Shop
{
    /// <summary>
    /// Scriptable object containing the the prices of an specific upgrade
    /// </summary>
    [CreateAssetMenu(menuName="UpgradePrices", fileName = "UpgradePrices")]
    public class SOUpgradePrices : ScriptableObject
    {
        public int[] prices;
    }
}