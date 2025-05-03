using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class GameplayUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject pausePanel;

        public void OnPausePressed()
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
        }

        public void OnResumePressed()
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
        }

        public void OnRestartPressed()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void OnMainMenuPressed()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }
}