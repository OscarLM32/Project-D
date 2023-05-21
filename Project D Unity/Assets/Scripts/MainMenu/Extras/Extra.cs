using System;
using CoreSystems.SaveLoadSystem;
using TMPro;
using UnityEngine;

namespace MainMenu.Extras
{
    /// <summary>
    /// Class defining and object inside the extras-menu menu
    /// </summary>
    public class Extra : MonoBehaviour
    {
        public TextMeshProUGUI display;
        
        /// <summary>
        /// Price to unlock
        /// </summary>
        public int value;
        public string story;

        /// <summary>
        /// Logic to be executed when it is tapped
        /// </summary>
        public void OnExtraTap()
        {
            ExtrasPresenter.ExtraSelected?.Invoke(story);
        }

        /// <summary>
        /// Handles the display of the missing "points" to unlock this extra
        /// </summary>
        public void DisplayNeededClassifiedDocuments()
        {
            var obtained = SaveLoadManager.Instance.GetClassifiedDocuments();
            if (obtained >= value)
            {
                display.text = "OBTAINED!";
            }
            else
            {
                display.text = obtained+"/"+value;
            }
        }
    }
}