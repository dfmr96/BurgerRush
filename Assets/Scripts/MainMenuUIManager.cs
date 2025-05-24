using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject startGamePanel;
    [SerializeField] private GameObject statsPanel;

    public void OnPlayPressed()
    {
        startGamePanel.SetActive(true);
    }

    public void OnStartGamePressed()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void OnCancelStartGame()
    {
        startGamePanel.SetActive(false);
    }

    public void OnStatsPressed()
    {
        statsPanel.SetActive(true);
    }

    public void OnCloseStatsPressed()
    {
        statsPanel.SetActive(false);
    }
    
    public void OnLoginButtonPressed()
    {
        if (GooglePlayAuthenticator.Instance != null)
        {
            GooglePlayAuthenticator.Instance.RetryManualSignIn();
        }
        else
        {
            Debug.LogError("GooglePlayAuthenticator not found!");
        }
    }

}