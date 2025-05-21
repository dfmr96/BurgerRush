using System;
using System.Threading.Tasks;
using Databases;
using Save;
using UnityEngine;

namespace Services.Cloud
{
    public static class CloudSyncService
    {
        public static event Action<int> OnCloudSyncStatusChanged;

        public static async Task ValidateCloudSync(PlayerStatsDatabase statsDB)
        {
            int syncResult = -1;

            try
            {
                bool isFreshInstall = PlayerStatsSaveService.IsFreshInstall();
                var localWrapper = PlayerStatsSaveService.ExportWrapperWithChecksum(statsDB);
                string cloudJson = await CloudSaveEntity<string>.Load("PlayerStats");

                if (string.IsNullOrEmpty(cloudJson))
                {
                    Debug.Log("☁️ No cloud data found. Saving local data...");
                    await CloudSaveStatsHandler.SaveStatsToCloud(statsDB);
                    PlayerStatsSaveService.MarkAsSaved();
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
                        PlayerStatsSaveService.MarkAsSaved();
                        syncResult = 1;
                    }
                    else if (!localValid)
                    {
                        Debug.LogWarning("⚠️ Local data is corrupted. Restoring cloud backup...");
                        PlayerStatsSaveService.ValidateAndImportWrapper(cloudWrapper, statsDB);
                        PlayerStatsSaveService.MarkAsSaved();
                        syncResult = 1;
                    }
                    else
                    {
                        string localChecksum = PlayerStatsSaveService.GenerateChecksum(localWrapper);
                        string cloudChecksum = PlayerStatsSaveService.GenerateChecksum(cloudWrapper);
                        bool checksumsMatch = localChecksum == cloudChecksum;

                        if (isFreshInstall)
                        {
                            Debug.Log("☁️ Fresh install. Using cloud data.");
                            PlayerStatsSaveService.ValidateAndImportWrapper(cloudWrapper, statsDB);
                            PlayerStatsSaveService.MarkAsSaved();
                            syncResult = 1;
                        }
                        else if (checksumsMatch)
                        {
                            Debug.Log("✅ Data and checksums match. No sync needed.");
                            syncResult = 1;
                        }
                        else
                        {
                            int cloudSeconds = PlayerStatsSaveService.GetTotalPlayTimeFromWrapper(cloudWrapper);
                            int localSeconds = PlayerStatsSaveService.GetTotalPlayTimeFromWrapper(localWrapper);

                            if (localSeconds > cloudSeconds)
                            {
                                Debug.Log("💾 Local has more playtime. Syncing to cloud...");
                                await CloudSaveStatsHandler.SaveStatsToCloud(statsDB);
                            }
                            else
                            {
                                Debug.Log("☁️ Cloud has more playtime. Syncing to local...");
                                PlayerStatsSaveService.ValidateAndImportWrapper(cloudWrapper, statsDB);
                                PlayerStatsSaveService.MarkAsSaved();
                            }

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
    }
}