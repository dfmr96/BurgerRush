using Services;
using Services.Cloud;
using TMPro;
using UnityEngine;

public class CloudSyncUIStatusUpdater : MonoBehaviour
{
    [SerializeField] private TMP_Text syncStatusText;

    private void Start()
    {
        CloudSyncService.OnCloudSyncStatusChanged += UpdateSyncStatusUI;

        int savedStatus = PlayerPrefs.GetInt("LastCloudSyncSuccess", -1);
        UpdateSyncStatusUI(savedStatus);
    }

    private void OnDestroy()
    {
        CloudSyncService.OnCloudSyncStatusChanged -= UpdateSyncStatusUI;
    }

    private void UpdateSyncStatusUI(int status)
    {
        switch (status)
        {
            case 1:
                syncStatusText.text = "<color=green>Synced</color>";
                break;
            case 0:
                syncStatusText.text = "<color=red>Not Synced</color>";
                break;
            default:
                syncStatusText.text = "<color=yellow>? Sync Unknown</color>";
                break;
        }
    }
}