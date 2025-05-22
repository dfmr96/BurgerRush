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
            int syncResult;

            try
            {
                bool isFreshInstall = PlayerStatsSaveService.IsFreshInstall();
                var localWrapper = PlayerStatsSaveService.CreateWrapperWithChecksum(statsDB);
                string cloudJson = await CloudSaveEntity<string>.Load("PlayerStats");

                if (string.IsNullOrEmpty(cloudJson))
                {
                    syncResult = await HandleNoCloudData(statsDB);
                }
                else
                {
                    var cloudWrapper = JsonUtility.FromJson<PlayerStatsSaveWrapper>(cloudJson);
                    syncResult = await HandleExistingCloudData(localWrapper, cloudWrapper, statsDB, isFreshInstall);
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

        private static async Task<int> HandleNoCloudData(PlayerStatsDatabase db)
        {
            Debug.Log("☁️ No cloud data found. Saving local data...");
            await CloudSaveStatsHandler.SaveStatsToCloud(db);
            PlayerStatsSaveService.MarkAsSaved();
            return 1;
        }

        private static async Task<int> HandleExistingCloudData(
            PlayerStatsSaveWrapper localWrapper,
            PlayerStatsSaveWrapper cloudWrapper,
            PlayerStatsDatabase db,
            bool isFreshInstall)
        {
            bool cloudValid = PlayerStatsSaveService.ValidateChecksum(cloudWrapper);
            bool localValid = PlayerStatsSaveService.ValidateChecksum(localWrapper);

            if (!cloudValid && !localValid)
            {
                Debug.LogError("❌ Both cloud and local data have invalid checksums.");
                return 0;
            }

            if (!cloudValid)
            {
                Debug.LogWarning("⚠️ Cloud data is corrupted. Saving local backup...");
                await CloudSaveStatsHandler.SaveStatsToCloud(db);
                PlayerStatsSaveService.MarkAsSaved();
                return 1;
            }

            if (!localValid)
            {
                Debug.LogWarning("⚠️ Local data is corrupted. Restoring cloud backup...");
                PlayerStatsSaveService.ValidateAndApplyWrapper(cloudWrapper, db);
                PlayerStatsSaveService.MarkAsSaved();
                return 1;
            }

            return await ResolveConflict(localWrapper, cloudWrapper, db, isFreshInstall);
        }

        private static async Task<int> ResolveConflict(
            PlayerStatsSaveWrapper localWrapper,
            PlayerStatsSaveWrapper cloudWrapper,
            PlayerStatsDatabase db,
            bool isFreshInstall)
        {
            string localChecksum = PlayerStatsSaveService.GenerateChecksum(localWrapper.data);
            string cloudChecksum = PlayerStatsSaveService.GenerateChecksum(cloudWrapper.data);
            bool checksumsMatch = localChecksum == cloudChecksum;

            if (isFreshInstall)
            {
                Debug.Log("☁️ Fresh install. Using cloud data.");
                PlayerStatsSaveService.ValidateAndApplyWrapper(cloudWrapper, db);
                PlayerStatsSaveService.MarkAsSaved();
                return 1;
            }

            if (checksumsMatch)
            {
                Debug.Log("✅ Data and checksums match. No sync needed.");
                return 1;
            }

            int cloudSeconds = PlayerStatsSaveService.GetTotalPlayTimeFromWrapper(cloudWrapper);
            int localSeconds = PlayerStatsSaveService.GetTotalPlayTimeFromWrapper(localWrapper);

            if (localSeconds > cloudSeconds)
            {
                Debug.Log("💾 Local has more playtime. Syncing to cloud...");
                await CloudSaveStatsHandler.SaveStatsToCloud(db);
            }
            else
            {
                Debug.Log("☁️ Cloud has more playtime. Syncing to local...");
                PlayerStatsSaveService.ValidateAndApplyWrapper(cloudWrapper, db);
                PlayerStatsSaveService.MarkAsSaved();
            }

            return 1;
        }
    }
}
