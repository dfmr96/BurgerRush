using System;
using Databases;
using Enums;
using Services;
using Services.Cloud;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private SFXType mainMenuThemeSFX;
    [SerializeField] private PlayerStatsDatabase statsDB;

    public static event Action<int> OnCloudSyncStatusChanged;

    private void Start()
    {
        AudioManager.Instance.PlayBackgroundMusic(mainMenuThemeSFX);
        UGSInitializer.OnUGSReady += ValidateCloudSync;
    }

    private async void ValidateCloudSync()
    {
        int syncResult = -1;

        try
        {
            string localJson = PlayerStatsSaveService.ExportWithChecksum(statsDB);
            string cloudJson = await CloudSaveEntity<string>.Load("PlayerStats");

            if (string.IsNullOrEmpty(cloudJson))
            {
                Debug.Log("☁️ No cloud data found. Saving local data...");
                await CloudSaveStatsHandler.SaveStatsToCloud(statsDB);
                syncResult = 1;
            }
            else if (PlayerStatsSaveService.CompareJsonChecksums(localJson, cloudJson))
            {
                Debug.Log("✅ Cloud data is up to date with local data.");
                syncResult = 1;
            }
            else
            {
                Debug.LogWarning("⚠️ Cloud and local data differ. Overwriting cloud with local.");
                await CloudSaveStatsHandler.SaveStatsToCloud(statsDB);
                syncResult = 1;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Failed to validate cloud sync: {e.Message}");
            syncResult = 0;
        }

        PlayerPrefs.SetInt("LastCloudSyncSuccess", syncResult);
        PlayerPrefs.Save();
        OnCloudSyncStatusChanged?.Invoke(syncResult);
    }

    private void OnDestroy()
    {
        UGSInitializer.OnUGSReady -= ValidateCloudSync;
    }
}