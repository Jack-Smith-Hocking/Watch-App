using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace WatchApp
{
    public class ClockManager : MonoBehaviour
    {
        public WatchManager m_manager;

        [Header("Settings")]
        public GameObject m_clockSection;
        public GameObject m_settingsSection;
        public Button m_settingsButton;
        public Button m_settingsExitButton;
        [Space]
        public TMP_Dropdown m_dateFormatDropdown;

        [Header("Text")]
        public TextMeshProUGUI m_timeText;
        public TextMeshProUGUI m_dateText;

        [Header("Clock Hands")]
        public Transform m_handAnchor;
        [Space]
        public Transform m_hourHand;
        public Transform m_minuteHand;
        [Space]
        public Transform m_secondHand;
        public Transform m_secondHandImposter;

        private System.DateTime m_currentDateTime;
        private bool m_is24Format = false;

        private string m_dateFormat = "dd / MM / yy";

        private void Start()
        {
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

                MoveHand(m_hourHand, _hour, 12);
                MoveHand(m_minuteHand, _min, 60);
                MoveHand(m_secondHand, _sec, 60);

                if (m_secondHand && m_secondHandImposter)
                {
                    float _dist = Vector2.Distance(m_secondHandImposter.position, m_handAnchor.position);

                    m_secondHand.position = m_handAnchor.position;
                    m_secondHandImposter.position = m_handAnchor.position + (m_secondHand.up * _dist);
                }
            }
        }

        /// <summary>
        /// Calculate the position and direction of a clock hand
        /// </summary>
        /// <param name="handTrans">The hand to change directions</param>
        /// <param name="time">The amount of time into this hand's cycle</param>
        /// <param name="maxTime">The max time for this hand's cycle (60 sec for a min)</param>
        void MoveHand(Transform handTrans, int time, int maxTime)
        {
            if (handTrans)
            {
                float _dist = Vector2.Distance(handTrans.position, m_handAnchor.position);
                float _timeRatio = time / (float)maxTime;

                // Multiply angle by -1 because it goes round the circle left side not right
                handTrans.eulerAngles = new Vector3(0, 0, 360 * _timeRatio * -1);
                handTrans.position = m_handAnchor.position + (handTrans.up * _dist);
            }
        }
    }
}