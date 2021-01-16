using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Utility.String;

namespace WatchApp
{
    public class WatchManager : MonoBehaviour
    {
        [Header("Section Buttons")]
        [Tooltip("Button to open the 'Clock' section")] public Button m_clockButton;
        [Tooltip("Button to open the 'Stopwatch' section")] public Button m_stopwatchButton;
        [Tooltip("Button to open the 'Timer' section")] public Button m_timerButton;

        [Header("Sections")]
        [Tooltip("Main watch section")] public GameObject m_mainSection;
        [Tooltip("The section that let's you add a watch")] public GameObject m_addSection;
        [Space]
        [Tooltip("The 'Clock' section")] public GameObject m_clockSection;
        [Tooltip("The 'Stopwatch' section")] public GameObject m_stopwatchSection;
        [Tooltip("The 'Timer' section")] public GameObject m_timerSection;

        [Header("Add / Remove")]
        [Tooltip("Button that adds a watch")] public Button m_addButton;
        [Tooltip("Button that removes a watch")] public Button m_removeButton;

        // Section Texts //
        private TextMeshProUGUI m_clockText;
        private TextMeshProUGUI m_stopwatchText;
        private TextMeshProUGUI m_timerText;

        // Start is called before the first frame update
        void Start()
        {
            InitButton(m_addButton, () =>
            {
                CloseSection(m_addSection, null, "");
                OpenSection(m_mainSection, null);
                OpenSection(m_clockSection, m_clockText);
            });
            InitButton(m_removeButton, () =>
            {
                CloseSection(m_mainSection, null, "");
                OpenSection(m_addSection, null);
            });

            InitButton(m_clockButton, ref m_clockText, () => { OpenClock(); });
            InitButton(m_stopwatchButton, ref m_stopwatchText, () => { OpenStopwatch(); });
            InitButton(m_timerButton, ref m_timerText, () => { OpenTimer(); });
        }

        // Update is called once per frame
        void Update()
        {

        }

        #region Public Open and Closers
        public void CloseClock()
        {
            CloseSection(m_clockSection, m_clockText, "Clock");
        }
        public void CloseStopwatch()
        {
            CloseSection(m_stopwatchSection, m_stopwatchText, "Stopwatch");
        }
        public void CloseTimer()
        {
            CloseSection(m_timerSection, m_timerText, "Timer");
        }

        public void OpenClock()
        {
            OpenSection(m_clockSection, m_clockText);
        }
        public void OpenStopwatch()
        {
            OpenSection(m_stopwatchSection, m_stopwatchText);
        }
        public void OpenTimer()
        {
            OpenSection(m_timerSection, m_timerText);
        }
        #endregion

        /// <summary>
        /// Closes all the sections then opens the correct one
        /// </summary>
        /// <param name="section">The section to open</param>
        public void OpenSection(GameObject section, TextMeshProUGUI textObj)
        {
            if (section)
            {
                // Make sure they're all closed
                CloseClock();
                CloseStopwatch();
                CloseTimer();

                // Open the desired one
                section.SetActive(true);

                if (textObj)
                {
                    textObj.color = Color.red;

                    // fontStyle is an bit structured enum, meaning to get multiple styles you have to OR them
                    textObj.fontStyle = FontStyles.Underline | FontStyles.Bold;
                }
            }
        }

        /// <summary>
        /// Closes the specified section if non-null
        /// </summary>
        /// <param name="section">The section to be closed</param>
        /// <param name="textObj">Reset the textObj to white and bold</param>
        /// <param name="text">The text to set it as</param>
        public static void CloseSection(GameObject section, TextMeshProUGUI textObj, string text)
        {
            if (section)
            {
                section.SetActive(false);

                if (textObj)
                {
                    textObj.text = text;
                    textObj.color = Color.white;

                    // fontStyle is an bit structured enum, meaning to get multiple styles you have to OR them
                    textObj.fontStyle = FontStyles.Normal | FontStyles.Bold;
                }
            }
        }

        /// <summary>
        /// Set up a button with a listener
        /// </summary>
        /// <param name="button">The button to add the listener to</param>
        /// <param name="listener">Action to invoke on click callback</param>
        public static void InitButton(Button button, System.Action listener)
        {
            if (button && listener != null)
            {
                button.onClick.AddListener(() => { listener?.Invoke(); });
            }
        }
        /// <summary>
        /// Set up a button and get its text component
        /// </summary>
        /// <param name="button">The button to add the listener to</param>
        /// <param name="buttonTextHolder">The text object to set the button text to</param>
        /// <param name="listener">Action to invoke on click callback</param>
        public static void InitButton(Button button, ref TextMeshProUGUI buttonTextHolder, System.Action listener)
        {
            if (button && listener != null)
            {
                button.onClick.AddListener(() => { listener?.Invoke(); });

                buttonTextHolder = button.GetComponentInChildren<TextMeshProUGUI>();
            }
        }
    }
}