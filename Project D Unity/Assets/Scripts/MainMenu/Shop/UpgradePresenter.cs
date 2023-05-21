using CoreSystems.SaveLoadSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.Shop
{
    /// <summary>
    /// Presenter of the PMV for one upgrade in the shop
    /// </summary>
    public class UpgradePresenter : MonoBehaviour
    {
        public UpgradeType type;
        public SOUpgradePrices upgradePrices;

        [Space]
        public Button upgradeButton;
        public TextMeshProUGUI upgradeText;

        private int _upgradesBought = 0;

        private void Start()
        {
            _upgradesBought = SaveLoadManager.Instance.GetTimesUpgradeWasBought(type);
            UpdateText();
        }

        /// <summary>
        /// Updates the text to match the upgrades bought and prices
        /// </summary>
        private void UpdateText()
        {
            if (_upgradesBought == upgradePrices.prices.Length)
            {
                upgradeButton.interactable = false;
                upgradeText.text = "MAX!";
                return;
            }
            upgradeText.text = upgradePrices.prices[_upgradesBought].ToString();
        }

        /// <summary>
        /// Logic to be executed when the buy button is pressed
        /// </summary>
        public void BuyUpgrade()
        {
            var currentPrice = upgradePrices.prices[_upgradesBought];
            if (currentPrice > SaveLoadManager.Instance.GetInformation())
            {
                return;
            }
            
            SaveLoadManager.Instance.BuyUpgrade(type, currentPrice);
            _upgradesBought++; 
            UpdateText();
        }
    }
}