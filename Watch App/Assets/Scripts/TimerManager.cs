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
        public GameObject m_inputSection;
        public GameObject m_timerSection;

        [Header("Input Section")]
        public Button m_startButton;
        public Button m_resetButton;
        [Space]
        public TMP_InputField m_hourInputField;
        public TMP_InputField m_minInputField;
        public TMP_InputField m_secInputField;

        [Header("Timer Section")]
        public Button m_playButton;
        public Button m_pauseButton;
        public Button m_stopButton;
        [Space]
        public TextMeshProUGUI m_timerText;

        [Header("Fill Image - Timer Section")]
        public Image m_timerFillImage;
        public float m_fillSpeed = 10;

        [Header("On Timer End")]
        public UnityEvent m_onTimerEndEvent;
        public AudioClip m_timerEndClip;

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
                    m_timeHour = GetInputFieldInt(m_hourInputField);
                    m_timeMin = GetInputFieldInt(m_minInputField);
                    m_timeSec = GetInputFieldInt(m_secInputField);

                    m_startTime = GetTimeInSeconds();

                    bool _validTimer = (m_timeHour + m_timeMin + m_timeSec) > 0;

                    if (m_timerSection && _validTimer)
                    {
                        m_trackingTimer = true;

                        m_timerSection.SetActive(true);

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

                if (m_timerText)
                {
                    m_timerText.text = $"{m_timeHour.ToString("00")}:";
                    m_timerText.text += $"{m_timeMin.ToString("00")}";
                    m_timerText.text += StringUtil.SetColour($".{m_timeSec.ToString("00")}", Color.red);
                }
                if (m_timerFillImage)
                {
                    float _fillAmount = m_currentTime / (float)m_startTime;

                    m_timerFillImage.fillAmount = Mathf.Lerp(m_timerFillImage.fillAmount, _fillAmount, m_fillSpeed * Time.deltaTime);
                }
            }
        }

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

        void TimerComplete()
        {
            if (m_trackingTimer)
            {
                m_trackingTimer = false;

                m_onTimerEndEvent.Invoke();

                if (m_audioSource)
                {
                    m_audioSource.PlayOneShot(m_timerEndClip);
                }
            }
        }

        int GetTimeInSeconds()
        {
            return (m_timeHour * 360) + (m_timeMin * 60) + (int)m_timeSec;
        }

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
        public static void SetInputFieldText(TMP_InputField inputField, string text)
        {
            if (inputField)
            {
                inputField.text = text;
            }
        }
    }
}