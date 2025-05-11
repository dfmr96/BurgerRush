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

    private void Start()
    {
        AudioManager.Instance.PlayBackgroundMusic(mainMenuThemeSFX);
        UGSInitializer.OnUGSReady += ValidateCloudSync;
    }

    private async void ValidateCloudSync()
    {
        string localJson = PlayerStatsSaveService.ExportWithChecksum(statsDB);
        string cloudJson = await CloudSaveEntity<string>.Load("PlayerStats");

        if (string.IsNullOrEmpty(cloudJson))
        {
            Debug.Log("☁️ No cloud data found. Saving local data...");
            await CloudSaveEntity<string>.Save("PlayerStats", localJson);
            return;
        }

        bool areSame = PlayerStatsSaveService.CompareJsonChecksums(localJson, cloudJson);
        if (areSame)
        {
            Debug.Log("✅ Cloud data is up to date with local data.");
        }
        else
        {
            Debug.LogWarning("⚠️ Cloud and local data differ. Overwriting cloud with local.");
            await CloudSaveEntity<string>.Save("PlayerStats", localJson);
        }
    }

    private void OnDestroy()
    {
        UGSInitializer.OnUGSReady -= ValidateCloudSync;
    }
}