using System;
using System.Collections.Generic;
using CoreSystems.MenuSystem;
using CoreSystems.SaveLoadSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace MainMenu.Extras
{
    /// <summary>
    /// Presenter of the PMV for the extras menu
    /// </summary>
    public class ExtrasPresenter : MonoBehaviour
    {
        public static Action<string> ExtraSelected;
        
        public SOExtrasInformation extrasInformation;

        /// <summary>
        /// Page where the extra is presented
        /// </summary>
        [Space]
        public GameObject extraPage;
        public TextMeshProUGUI extraStory;

        [Space]
        [SerializeField]private List<GameObject> _extras = new();

        private void Start()
        {
            PopulateExtras();
            SetUpExtrasInformation();
            SetUpExtrasTitles();
            CheckExtrasInteractable();
        }

        private void OnEnable()
        {
            ExtraSelected += OnExtraSelected;
            MainMenuEvents.LocaleChanged += OnLocaleChange;
        }

        private void OnDisable()
        {
            ExtraSelected -= OnExtraSelected; 
            //MainMenuEvents.LocaleChanged -= OnLocaleChange; 
        }

        private void OnDestroy()
        {
            MainMenuEvents.LocaleChanged -= OnLocaleChange; 
        }

        /// <summary>
        /// Populates the list with all the Extras in the menu
        /// </summary>
        private void PopulateExtras()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var extra = transform.GetChild(i).GetComponent<Extra>();
                if (extra)
                {
                    _extras.Add(transform.GetChild(i).gameObject);
                }
            }
        }

        /// <summary>
        /// Sets up every extra in the menu with the corresponding information and value
        /// </summary>
        private void SetUpExtrasInformation()
        {
            for (int i = 0; i < _extras.Count; i++)
            {
                var extra = _extras[i].GetComponent<Extra>();
                LocalizedString table = new LocalizedString
                {
                    TableReference = StringTables.Dynamic_UI.ToString(),
                    TableEntryReference = "EXTRA_STORY_"+(i+1),
                    WaitForCompletion = true
                }; 
                
                var story = table.GetLocalizedString();
                extra.value = extrasInformation.values[i];
                extra.story = story;
            }
        }

        private void SetUpExtrasTitles()
        {
            LocalizedString table = new LocalizedString
            {
                TableReference = StringTables.Dynamic_UI.ToString(),
                TableEntryReference = "EXTRA_TITLE"
            };
            var titleText = table.GetLocalizedString();
            
            for (int i = 0; i < _extras.Count; i++)
            {
                //TODO: there must be a better way to access the title
                var title = _extras[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                title.text = titleText + i;
            }
        }

        /// <summary>
        /// Checks if the extra is interactable
        /// </summary>
        private void CheckExtrasInteractable()
        {
            int classifiedDocuments = SaveLoadManager.Instance.GetClassifiedDocuments();

            foreach (var extraObject in _extras)
            {
                var extra = extraObject.GetComponent<Extra>();
                if (classifiedDocuments < extra.value)
                {
                    extraObject.GetComponent<Button>().interactable = false;
                }
                extra.DisplayNeededClassifiedDocuments();
            }
        }

        /// <summary>
        /// Logic to be executed when an extra is tapped
        /// </summary>
        /// <param name="story"></param>
        private void OnExtraSelected(string story)
        {
            extraStory.text = story;
            PageController.instance.TurnPageOn(PageType.EXTRA);
        }

        private void OnLocaleChange()
        {
            SetUpExtrasInformation();
            SetUpExtrasTitles();
            foreach (var extraObject in _extras)
            {
                var extra = extraObject.GetComponent<Extra>();
                extra.DisplayNeededClassifiedDocuments();
            }
        }
    }
}