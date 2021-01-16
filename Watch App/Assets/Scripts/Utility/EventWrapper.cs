using UnityEngine;
using UnityEngine.SceneManagement;

namespace Custom.Utility
{
    public class EventWrapper : MonoBehaviour
    {
        public void SetTmeScale(float timeScale)
        {
            Time.timeScale = timeScale;
        }

        public void ToggleScript(MonoBehaviour mono)
        {
            if (mono != null)
            {
                mono.enabled = !mono.enabled;
            }
        }

        public void ToggleTarget(GameObject target)
        {
            if (target != null)
            {
                target.SetActive(!target.activeInHierarchy);
            }
        }
        public void ToggleSelf()
        {
            ToggleTarget(gameObject);
        }

        public void LoadSceneIndex(int index)
        {
            if (index > 0 && index <= SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(index);
            }
        }
        public void LoadSceneName(string sceneName)
        {
            if (sceneName.Length == 0) return;

            SceneManager.LoadScene(sceneName);
        }

        public void ReloadScene() 
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void DestroyTarget(GameObject target)
        {
            if (target != null)
            {
                Destroy(target);
            }
        }

        public void DestroySelf()
        {
            DestroyTarget(gameObject);
        }

        public void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        public void ConfineCursor()
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        public void ResetCursor()
        {
            Cursor.lockState = CursorLockMode.None;
        }
        public void SetCursorVisibility(bool isVisible)
        {
            Cursor.visible = isVisible;
        }
    }
}