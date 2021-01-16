using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Utility.String;

namespace WatchApp
{
    public class StopwatchManager : MonoBehaviour
    {
        [Header("Timer Details")]
        [Tooltip("The text to display the timer")] public TextMeshProUGUI m_timerText;
        [Space]
        [Tooltip("The button that will start the timer")] public Button m_startButton;
        [Tooltip("The button that will unpause the timer")] public Button m_playButton;
        [Tooltip("The button that will pause the timer")] public Button m_pauseButton;
        [Tooltip("The button that will reset the timer")] public Button m_resetButton;

        [Header("Timer Hand")]
        [Tooltip("Anchor for the timer hand")] public Transform m_anchor;
        [Space]
        [Tooltip("The clock hand")] public Transform m_timerHand;
        [Tooltip("Clock hand imposter")] public Transform m_timerHandImposter;

        private float m_handDist;
        private float m_currentTime;

        private int m_timeMin;
        private int m_timeSec;
        private int m_timeMilSec;

        private bool m_isTracking = false;

        // Start is called before the first frame update
        void Start()
        {
            if (m_timerHandImposter && m_anchor)
            {
                m_handDist = Vector2.Distance(m_timerHandImposter.position, m_anchor.position);
            }

            // Initiate all buttons
            WatchManager.InitButton(m_startButton, () => 
            {
                m_isTracking = true;

                m_startButton.gameObject.SetActive(false);

                if (m_pauseButton)
                {
                    m_pauseButton.gameObject.SetActive(true);
                }
                if (m_resetButton)
                {
                    m_resetButton.gameObject.SetActive(true);
                }
            });
            WatchManager.InitButton(m_playButton, () =>
            {
                m_isTracking = true;

                if (m_pauseButton)
                {
                    m_pauseButton.gameObject.SetActive(true);
                    m_playButton.gameObject.SetActive(false);
                }
            });
            WatchManager.InitButton(m_pauseButton, () =>
            {
                m_isTracking = false;

                if (m_playButton)
                {
                    m_playButton.gameObject.SetActive(true);
                    m_pauseButton.gameObject.SetActive(false);
                }
            });
            WatchManager.InitButton(m_resetButton, () =>
            {
                m_isTracking = false;

                ResetStopwatch();
            });
        }

        // Update is called once per frame
        void Update()
        {
            if (m_isTracking)
            {
                m_currentTime += Time.deltaTime;

                if (m_currentTime >= 0.1f)
                {
                    m_currentTime -= 0.1f;
                    m_timeMilSec++;
                }

                // Update the second and minute timers
                UpdateTime(ref m_timeSec, ref m_timeMilSec, 10);
                UpdateTime(ref m_timeMin, ref m_timeSec, 60);

                // Update timer text
                UpdateTimer();
                UpdateHand();
            }
        }

        /// <summary>
        /// Check if a timer is over a threshold and then update it
        /// </summary>
        /// <param name="time">The time to add to if over a threshold</param>
        /// <param name="checkTime">The time to check for</param>
        /// <param name="checkMin">The time threshold</param>
        void UpdateTime(ref int time, ref int checkTime, int checkMin)
        {
            if (checkTime >= checkMin)
            {
                checkTime -= checkMin;
                time++;
            }
        }

        /// <summary>
        /// Update the text that displays the timer
        /// </summary>
        void UpdateTimer()
        {
            if (m_timerText)
            {
                m_timerText.text = $"{m_timeMin.ToString("00")}:";
                m_timerText.text += $"{m_timeSec.ToString("00")}";
                m_timerText.text += StringUtil.SetColour($".{m_timeMilSec.ToString("0")}", Color.red);
            }
        }
        void UpdateHand()
        {
            // Use code written for clock hands for the stopwatch hand
            ClockManager.MoveHand(m_timerHand, m_anchor, m_timeSec, 60, m_handDist);

            if (m_timerHandImposter)
            {
                Vector3 _pos = m_anchor.position + (m_timerHand.up * m_handDist);

                m_timerHand.position = m_anchor.position;
                m_timerHandImposter.position = _pos;
            }
        }

        /// <summary>
        /// Will reset timer to 0
        /// </summary>
        public void ResetStopwatch()
        {
            m_isTracking = false;

            m_currentTime = 0;

            m_timeMin = 0;
            m_timeSec = 0;
            m_timeMilSec = 0;

            if (m_startButton)
            {
                m_startButton.gameObject.SetActive(true);
            }
            if (m_playButton)
            {
                m_playButton.gameObject.SetActive(false);
            }
            if (m_pauseButton)
            {
                m_pauseButton.gameObject.SetActive(false);
            }
            if (m_resetButton)
            {
                m_resetButton.gameObject.SetActive(false);
            }

            UpdateTimer();
            UpdateHand();
        }
    }
}