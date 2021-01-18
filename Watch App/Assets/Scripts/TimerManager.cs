using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utility.String;
using UnityEngine.Events;

namespace WatchApp
{
    public class TimerManager : MonoBehaviour
    {
        public AudioSource m_audioSource;

        [Header("Sections")]
        [Tooltip("The section to input timer values")] public GameObject m_inputSection;
        [Tooltip("The section to display the timer")] public GameObject m_timerSection;

        [Header("Input Section")]
        [Tooltip("The button to start the timer")] public Button m_startButton;
        [Tooltip("The button to reset the timer")] public Button m_resetButton;
        [Space]
        [Tooltip("The InputField to get the hours for the timer")] public TMP_InputField m_hourInputField;
        [Tooltip("The InputField to get the minutes for the timer")] public TMP_InputField m_minInputField;
        [Tooltip("The InputField to get the seconds for the timer")] public TMP_InputField m_secInputField;

        [Header("Timer Section")]
        [Tooltip("The button to play the timer")] public Button m_playButton;
        [Tooltip("The button to pause the timer")] public Button m_pauseButton;
        [Tooltip("The button to stop the timer")] public Button m_stopButton;
        [Space]
        [Tooltip("The text to display the timer")] public TextMeshProUGUI m_timerText;

        [Header("Fill Image - Timer Section")]
        [Tooltip("The fillable image that goes down with the timer")] public Image m_timerFillImage;
        [Tooltip("How quickly the image fill should interpolate its fill")] public float m_fillSpeed = 10;

        [Header("On Timer End")]
        [Tooltip("Event to invoke when the timer completes")] public UnityEvent m_onTimerEndEvent;
        [Tooltip("The AudioClip to play when the timer completes")] public AudioClip m_timerEndClip;

        private int m_startTime;
        private float m_currentTime;

        private float m_completedPerc;

        private int m_timeHour;
        private int m_timeMin;
        private float m_timeSec;

        private bool m_trackingTimer = false;

        // Start is called before the first frame update
        void Start()
        {
            // Input Section Buttons //
            {
                WatchManager.InitButton(m_startButton, () =>
                {
                    // Get the timer values
                    m_timeHour = GetInputFieldInt(m_hourInputField);
                    m_timeMin = GetInputFieldInt(m_minInputField);
                    m_timeSec = GetInputFieldInt(m_secInputField);

                    // Convert the timer values into seconds
                    m_startTime = GetTimeInSeconds();

                    // Check if the input values were valid (above 0)
                    bool _validTimer = (m_timeHour + m_timeMin + m_timeSec) > 0;

                    if (m_timerSection && _validTimer)
                    {
                        m_trackingTimer = true;

                        m_timerSection.SetActive(true);

                        // Reset the input values
                        SetInputFieldText(m_hourInputField, string.Empty);
                        SetInputFieldText(m_minInputField, string.Empty);
                        SetInputFieldText(m_secInputField, string.Empty);

                        if (m_inputSection)
                        {
                            m_inputSection.SetActive(false);
                        }
                    }
                });
                WatchManager.InitButton(m_resetButton, () =>
                {
                    ResetTimer();
                });
            }

            // Timer Section Buttons //
            {
                WatchManager.InitButton(m_playButton, () =>
                {
                    if (GetTimeInSeconds() > 0)
                    {
                        m_trackingTimer = true;

                        m_playButton.gameObject.SetActive(false);
                        if (m_pauseButton)
                        {
                            m_pauseButton.gameObject.SetActive(true);
                        }
                    }
                });
                WatchManager.InitButton(m_pauseButton, () =>
                {
                    m_trackingTimer = false;

                    m_pauseButton.gameObject.SetActive(false);
                    if (m_playButton)
                    {
                        m_playButton.gameObject.SetActive(true);
                    }
                });
                WatchManager.InitButton(m_stopButton, () =>
                {
                    m_startTime = 0;
                    m_trackingTimer = false;

                    CloseTimerUI();
                });
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (m_trackingTimer)
            {
                CalcTime();

                m_currentTime = GetTimeInSeconds();

                m_timeSec -= Time.deltaTime;

                // Display the time with some magic string formatting
                // Format it to display as 06, 12 etc
                if (m_timerText)
                {
                    m_timerText.text = $"{m_timeHour.ToString("00")}:";
                    m_timerText.text += $"{m_timeMin.ToString("00")}";
                    m_timerText.text += StringUtil.SetColour($".{m_timeSec.ToString("00")}", Color.red);
                }
                // Update and interpolate the fill image
                if (m_timerFillImage)
                {
                    float _fillAmount = m_currentTime / (float)m_startTime;

                    m_timerFillImage.fillAmount = Mathf.Lerp(m_timerFillImage.fillAmount, _fillAmount, m_fillSpeed * Time.deltaTime);
                }
            }
        }

        /// <summary>
        /// Calculate the time, reduce hours to minutes and minutes to seconds when needed
        /// </summary>
        void CalcTime()
        {
            if (m_timeSec <= 0.5f)
            {
                if (m_timeMin > 0)
                {
                    m_timeMin--;
                    m_timeSec += 59;
                }
                else if (m_timeHour > 0)
                {
                    m_timeHour--;
                    m_timeMin += 59;
                    m_timeSec += 59;
                }
                else
                {
                    TimerComplete();
                }
            }
        }

        /// <summary>
        /// Close the timer UI and open the Input UI
        /// </summary>
        public void CloseTimerUI()
        {
            if (m_timerSection)
            {
                m_timerSection.SetActive(false);
            }
            if (m_inputSection)
            {
                m_inputSection.SetActive(true);
            }
        }

        /// <summary>
        /// When the timer completes, call this function
        /// </summary>
        void TimerComplete()
        {
            if (m_trackingTimer)
            {
                m_trackingTimer = false;

                m_onTimerEndEvent.Invoke();

                // Play a sound on completion
                if (m_audioSource)
                {
                    m_audioSource.PlayOneShot(m_timerEndClip);
                }
            }
        }

        /// <summary>
        /// Comvert the hours, minutes and seconds into pure seconds
        /// </summary>
        /// <returns></returns>
        int GetTimeInSeconds()
        {
            return (m_timeHour * 360) + (m_timeMin * 60) + (int)m_timeSec;
        }

        /// <summary>
        /// Reset the timer and associated values
        /// </summary>
        public void ResetTimer()
        {
            m_timeHour = 0;
            m_timeMin = 0;
            m_timeSec = 0;

            m_trackingTimer = false;

            SetInputFieldText(m_hourInputField, string.Empty);
            SetInputFieldText(m_minInputField, string.Empty);
            SetInputFieldText(m_secInputField, string.Empty);
        }

        /// <summary>
        /// Get the integer value from an InputField
        /// </summary>
        /// <param name="inputField">The InputField to get the integer from</param>
        /// <returns></returns>
        public static int GetInputFieldInt(TMP_InputField inputField)
        {
            if (inputField == null)
            {
                return 0;
            }

            if (!int.TryParse(inputField.text, out int _result))
            {
                _result = 0;
            }

            return _result;
        }
        /// <summary>
        /// Set the text of an InputField
        /// </summary>
        /// <param name="inputField">InputField to update</param>
        /// <param name="text">Text to update to</param>
        public static void SetInputFieldText(TMP_InputField inputField, string text)
        {
            if (inputField)
            {
                inputField.text = text;
            }
        }
    }
}