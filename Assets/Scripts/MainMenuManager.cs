using System;
using Databases;
using Enums;
using Save;
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
        AudioManager.Instance.SetMusicPitch(1f);
        PlayerStatsService.InitializeAllStats(statsDB);
        
#if !UNITY_WEBGL
    UGSInitializer.OnUGSReady += ValidateCloudSync;
#else
        Debug.Log("☁️ WebGL build: Cloud Save desactivado.");
        PlayerPrefs.SetInt("LastCloudSyncSuccess", 0); // o 1, si querés evitar mostrar warning
        PlayerPrefs.Save();
        OnCloudSyncStatusChanged?.Invoke(0);
#endif
    }

private async void ValidateCloudSync()
{
    int syncResult = -1;

    try
    {
        var localWrapper = PlayerStatsSaveService.ExportWrapperWithChecksum(statsDB);
        string cloudJson = await CloudSaveEntity<string>.Load("PlayerStats");

        if (string.IsNullOrEmpty(cloudJson))
        {
            Debug.Log("☁️ No cloud data found. Saving local data...");
            await CloudSaveStatsHandler.SaveStatsToCloud(statsDB);
            syncResult = 1;
        }
        else
        {
            var cloudWrapper = JsonUtility.FromJson<PlayerStatsSaveWrapper>(cloudJson);

            bool cloudValid = PlayerStatsSaveService.ValidateChecksum(cloudWrapper);
            bool localValid = PlayerStatsSaveService.ValidateChecksum(localWrapper);

            if (!cloudValid && !localValid)
            {
                Debug.LogError("❌ Both cloud and local data have invalid checksums.");
                syncResult = 0;
            }
            else if (!cloudValid)
            {
                Debug.LogWarning("⚠️ Cloud data is corrupted. Saving local backup...");
                await CloudSaveStatsHandler.SaveStatsToCloud(statsDB);
                syncResult = 1;
            }
            else if (!localValid)
            {
                Debug.LogWarning("⚠️ Local data is corrupted. Restoring cloud backup...");
                PlayerStatsSaveService.ValidateAndImportWrapper(cloudWrapper, statsDB);
                syncResult = 1;
            }
            else
            {
                // ✅ Compara los checksums completos (incluye timestamp)
                string localChecksum = PlayerStatsSaveService.GenerateChecksum(localWrapper);
                string cloudChecksum = PlayerStatsSaveService.GenerateChecksum(cloudWrapper);

                if (localChecksum == cloudChecksum)
                {
                    Debug.Log("✅ Cloud and local data are already synchronized.");
                    syncResult = 1;
                }
                else if (cloudWrapper.lastSavedAt > localWrapper.lastSavedAt)
                {
                    Debug.Log("☁️ Cloud data is newer. Syncing to local...");
                    PlayerStatsSaveService.ValidateAndImportWrapper(cloudWrapper, statsDB);
                    syncResult = 1;
                }
                else
                {
                    Debug.Log("💾 Local data is newer. Syncing to cloud...");
                    await CloudSaveStatsHandler.SaveStatsToCloud(statsDB);
                    syncResult = 1;
                }
            }
        }
    }
    catch (Exception e)
    {
        Debug.LogError($"❌ Sync error: {e.Message}");
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