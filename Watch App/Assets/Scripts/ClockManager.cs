using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace WatchApp
{
    public class ClockManager : MonoBehaviour
    {
        [Tooltip("The WatchManager that is managing this clock")] public WatchManager m_manager;

        [Header("Sections")]
        [Tooltip("The main clock section (not the root and not the settings section)")] public GameObject m_clockSection;
        [Tooltip("The setting panel")] public GameObject m_settingsSection;

        [Header("Settings")]
        [Tooltip("The button that will take you to the settings section")] public Button m_settingsButton;
        [Tooltip("The button that will take you out of the settings section")] public Button m_settingsExitButton;
        [Space]
        [Tooltip("The dropdown that has options for the date format")] public TMP_Dropdown m_dateFormatDropdown;

        [Header("Text")]
        [Tooltip("The text that displays the time")] public TextMeshProUGUI m_timeText;
        [Tooltip("The text that displays the date")] public TextMeshProUGUI m_dateText;

        [Header("Clock Hands")]
        [Tooltip("The centre of the clock face to anchor the hands to")] public Transform m_handAnchor;
        [Space]
        [Tooltip("The hand that displays the hour on the clock")] public Transform m_hourHand;
        [Tooltip("The hand that displays the minute on the clock")] public Transform m_minuteHand;
        [Space]
        [Tooltip("The hand that displays the second on the clock (Either use like the other hands or with an imposter)")] public Transform m_secondHand;
        [Tooltip("The actual icon for the second hand, this is for something that isn't directly attached to the centre point")] public Transform m_secondHandImposter;

        private System.DateTime m_currentDateTime;
        private bool m_is24Format = false;

        private string m_dateFormat = "dd / MM / yy";

        // For optimisation instead of calling Vector2.Distance(...) several time per frame
        private float m_hourDist = 0;
        private float m_minDist = 0;
        private float m_secDist = 0;

        private void Start()
        {
            if (m_handAnchor)
            {
                if (m_hourHand)
                {
                    m_hourDist = Vector2.Distance(m_hourHand.position, m_handAnchor.position);
                }
                if (m_minuteHand)
                {
                    m_minDist = Vector2.Distance(m_minuteHand.position, m_handAnchor.position);
                }
                if (m_secondHandImposter)
                {
                    m_secDist = Vector2.Distance(m_secondHandImposter.position, m_handAnchor.position);
                }
                else if (m_secondHand)
                {
                    m_secDist = Vector2.Distance(m_secondHand.position, m_handAnchor.position);
                }
            }

            // Set up the settings button to turn on the panel
            WatchManager.InitButton(m_settingsButton, () =>
            {
                if (m_settingsSection)
                {
                    m_settingsSection.SetActive(true);
                }
                if (m_clockSection)
                {
                    m_clockSection.SetActive(false);
                }
            });

            // Set up the settings exit button to turn off the panel
            WatchManager.InitButton(m_settingsExitButton, () =>
            {
                if (m_settingsSection)
                {
                    m_settingsSection.SetActive(false);
                }
                if (m_clockSection)
                {
                    m_clockSection.SetActive(true);
                }
            });

            // Set up the callback for the date dropdown to update the formatting
            if (m_dateFormatDropdown)
            {
                m_dateFormatDropdown.onValueChanged.AddListener((int index) =>
                {
                    SetDateFormat(m_dateFormatDropdown.options[index].text);
                });
            }
        }

        // Update is called once per frame
        void Update()
        {
            m_currentDateTime = System.DateTime.Now;

            UpdateHands();
            UpdateDateTimeText();
        }

        public void SetDateFormat(string dateFormat)
        {
            m_dateFormat = dateFormat;
        }
        public void Set24Format(bool is24Format)
        {
            m_is24Format = is24Format;
        }

        public void UpdateDateTimeText()
        {
            // == Optimisations ==
            // - Could optimise so that I'm not the text every frame
            // - Could store the time formatting similarly to the way I'm doing date formating
            if (m_timeText)
            {
                // Use string formatting to get 24hour or 12hour time
                m_timeText.text = m_currentDateTime.ToString(m_is24Format ? "H:mm" : "h:mm") + Utility.String.StringUtil.SetSize(m_currentDateTime.ToString(" tt"), 60);
            }

            if (m_dateText)
            {
                m_dateText.text = m_currentDateTime.ToString(m_dateFormat);
            }
        }

        /// <summary>
        /// Update position and direction of all clock hands
        /// </summary>
        void UpdateHands()
        {
            if (m_handAnchor)
            {
                int _hour = m_currentDateTime.Hour;
                int _min = m_currentDateTime.Minute;
                int _sec = m_currentDateTime.Second;

                // Calculate the position and direction of all the time hands
                MoveHand(m_hourHand, m_handAnchor, _hour, 12, m_hourDist);
                MoveHand(m_minuteHand, m_handAnchor, _min, 60, m_minDist);
                MoveHand(m_secondHand, m_handAnchor, _sec, 60, m_secDist);

                if (m_secondHand && m_secondHandImposter)
                {
                    m_secondHand.position = m_handAnchor.position;
                    m_secondHandImposter.position = m_handAnchor.position + (m_secondHand.up * m_secDist);
                }
            }
        }

        /// <summary>
        /// Calculate the position and direction of a clock hand
        /// </summary>
        /// <param name="handTrans">The hand to change directions</param>
        /// <param name="time">The amount of time into this hand's cycle</param>
        /// <param name="maxTime">The max time for this hand's cycle (60 sec for a min)</param>
        public static void MoveHand(Transform handTrans, Transform anchor, int time, int maxTime, float dist)
        {
            if (handTrans && anchor)
            {
                float _timeRatio = time / (float)maxTime;

                // Multiply angle by -1 because it goes round the circle left side not right
                handTrans.eulerAngles = new Vector3(0, 0, 360 * _timeRatio * -1);
                handTrans.position = anchor.position + (handTrans.up * dist);
            }
        }
    }
}