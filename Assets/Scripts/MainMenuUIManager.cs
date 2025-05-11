using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIManager: MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject startGamePanel;
    [SerializeField] private GameObject statsPanel;

    [Header("Cloud Sync Status")]
    [SerializeField] private TMP_Text syncStatusText;

    private void Start()
    {
        int savedStatus = PlayerPrefs.GetInt("LastCloudSyncSuccess", -1);
        UpdateSyncStatusUI(savedStatus);
        MainMenuManager.OnCloudSyncStatusChanged += UpdateSyncStatusUI;
    }

    private void OnDestroy()
    {
        MainMenuManager.OnCloudSyncStatusChanged -= UpdateSyncStatusUI;
    }

    private void UpdateSyncStatusUI(int status)
    {
        switch (status)
        {
            case 1:
                syncStatusText.text = "<color=green>✔ Synced</color>";
                break;
            case 0:
                syncStatusText.text = "<color=red>✖ Not Synced</color>";
                break;
            default:
                syncStatusText.text = "<color=yellow>? Sync Unknown</color>";
                break;
        }
    }

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
}