using CoreSystems.SaveLoadSystem;
using CoreSystems.SceneSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    /// <summary>
    /// Presenter for the PMV for the level selection
    /// </summary>
    public class LevelSelectionPresenter : MonoBehaviour
    {
        /// <summary>
        /// Button to select the previous level
        /// </summary>
        public Button leftButton; 
        
        /// <summary>
        /// Button to select the next level
        /// </summary>
        public Button rightButton;

        public TextMeshProUGUI levelText;

        private const int _maxReachableLevel = 3;
        
        private int _maxLevelReached;
        private int _selectedLevel = 1;

        private void Start()
        {
            _maxLevelReached = SaveLoadManager.Instance.GetMaxLevelReached();
            _selectedLevel = _maxLevelReached;

            if (_selectedLevel > _maxReachableLevel)
            {
                _selectedLevel = _maxReachableLevel;
            }
            
            UpdateView();
        }

        /// <summary>
        /// Selects the next level
        /// </summary>
        public void NextLevel()
        {
            _selectedLevel++;
            UpdateView();
        }

        /// <summary>
        /// Selects the previous level
        /// </summary>
        public void PreviousLevel()
        {
            _selectedLevel--;
            UpdateView();
        }

        /// <summary>
        /// Loads the level
        /// </summary>
        public void LoadLevel()
        {
            SceneController.instance.LoadLevel(_selectedLevel/*, loadingPage: PageType.LEVEL_LOAD*/);
        }

        /// <summary>
        /// Handles the logic behind updating the view
        /// </summary>
        private void UpdateView()
        {
            levelText.text = _selectedLevel.ToString();
            CheckButtonsInteractable();
        }

        /// <summary>
        /// Checks if the buttons should be interactable
        /// </summary>
        private void CheckButtonsInteractable()
        {
            leftButton.interactable = _selectedLevel != 1;

            rightButton.interactable = _selectedLevel != _maxLevelReached;
        }
    }
}