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
        public static Action UpdateInformation;
        
        public TextMeshProUGUI information;
        public TextMeshProUGUI classifiedDocuments;

        private void Start()
        {
            information.text = SaveLoadManager.Instance.GetInformation().ToString();
            classifiedDocuments.text = SaveLoadManager.Instance.GetClassifiedDocuments().ToString();
        }

        private void OnEnable()
        {
            UpdateInformation += OnInformationUpdate;
        }

        private void OnDisable()
        {
            UpdateInformation -= OnInformationUpdate;
        }

        /// <summary>
        /// Logic to be executed when the information value 
        /// </summary>
        private void OnInformationUpdate()
        {
            information.text = SaveLoadManager.Instance.GetInformation().ToString(); 
        }
    }
}