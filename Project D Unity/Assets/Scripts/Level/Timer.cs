using System.Collections;
using TMPro;
using UnityEngine;

namespace Level
{
    /// <summary>
    /// Handles the timer inside each level
    /// </summary>
    public class Timer : MonoBehaviour
    {
        /// <summary>
        /// The ui element of the timer
        /// </summary>
        public TextMeshProUGUI timerDisplay;
        
        private const int _minCompletionTime = 30;
        private int _totalTime = _minCompletionTime;
        private float _timeElapsed;

        /// <summary>
        /// Extra time given to the player not represented in the UI
        /// </summary>
        private const float _graceTime = 1f; //An extra time given to the player not reflected in the timer    

        private bool _timerStarted = false;

        private void Update()
        {
            if (!_timerStarted) return;

            _timeElapsed += Time.deltaTime;
            if (_timeElapsed > _totalTime + _graceTime)
            {
                _timerStarted = false;
                GameEvents.TimeRanOut?.Invoke();
            }

            if (_timeElapsed > _totalTime) return;
            
            UpdateTimer();
        }

        /// <summary>
        /// Set ups the timer with the proper values for the level
        /// </summary>
        /// <param name="extraTime">Teh amount of extra time given in this level (can be negative)</param>
        /// <param name="waitBeforeStart">The amount of time to wait before starting the timer</param>
        public void SetUpTimer(int extraTime, float waitBeforeStart = 0f)
        {
            _totalTime += extraTime;
            _timeElapsed = 0;
            UpdateTimer();

            StartCoroutine(StartTimer(waitBeforeStart));
        }

        private IEnumerator StartTimer(float wait)
        {
            yield return new WaitForSeconds(wait);

            _timerStarted = true;
        }

        /// <summary>
        /// Updates the timer data and the UI display
        /// </summary>
        private void UpdateTimer()
        {
            var remainingTime = _totalTime - _timeElapsed;
            var formattedTime = FormatRemainingTime(remainingTime);

            timerDisplay.text = formattedTime;
        }
        
        /// <summary>
        /// Formats the remaining time into the proper format of -> mm:ss
        /// </summary>
        /// <param name="remainingTime">The time remaining</param>
        /// <returns>The formatted time</returns>
        private string FormatRemainingTime(float remainingTime)
        {
            //Get rid of the floating point values
            remainingTime = (int) remainingTime;
            string minutes = ((int)(remainingTime / 60)).ToString();
            string seconds = ((int)(remainingTime % 60)).ToString();

            if (minutes.Length == 1)
            {
                minutes = minutes.Insert(0, "0");
            }

            if (seconds.Length == 1)
            {
                seconds = seconds.Insert(0, "0");
            }

            var finalTimer = $"{minutes} : {seconds}";
            return finalTimer;
        }
    }
}