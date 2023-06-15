using System;
using CoreSystems.SaveLoadSystem;
using TMPro;
using UnityEngine;

namespace MainMenu
{
    /// <summary>
    /// Presenter for the PMV of the currency
    /// </summary>
    public class CurrencyPresenter : MonoBehaviour
    {
        public static Action<int> UpdateCurrencyPresentation;
        
        public TextMeshProUGUI information;
        public TextMeshProUGUI classifiedDocuments;

        private void Start()
        {
            information.text = SaveLoadManager.Instance.GetInformation().ToString();
            classifiedDocuments.text = SaveLoadManager.Instance.GetClassifiedDocuments().ToString();
        }

        private void OnEnable()
        {
            UpdateCurrencyPresentation += OnInformationUpdate;
        }

        private void OnDisable()
        {
            UpdateCurrencyPresentation -= OnInformationUpdate;
        }

        /// <summary>
        /// Logic to be executed when the information value 
        /// </summary>
        private void OnInformationUpdate(int newValue)
        {
            information.text = newValue.ToString();
        }
    }
}