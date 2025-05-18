using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameplayUIManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionPanel;
    public void OnPausePressed()
    {
        Time.timeScale = 0f;
        EventSystem.current.SetSelectedGameObject(null);
        Debug.Log("Pause Pressed");
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
        
    public void OnOptionPressed()
    {
        Debug.Log("Option Pressed");
        optionPanel.SetActive(true);
        Canvas.ForceUpdateCanvases();
    }
}