using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Databases;
using Save;
using Unity.Services.CloudSave;
using UnityEngine;

namespace Services.Cloud
{
    public static class CloudSaveStatsHandler
    {
        private const string CloudKey = "PlayerStats";

        public static async Task SaveStatsToCloud(PlayerStatsDatabase db)
        {
            var wrapper = PlayerStatsSaveService.ExportWrapperWithChecksum(db);
            await CloudSaveEntity<PlayerStatsSaveWrapper>.Save(CloudKey, wrapper);
        }

        public static async Task<bool> LoadStatsFromCloud(PlayerStatsDatabase db)
        {
            var wrapper = await CloudSaveEntity<PlayerStatsSaveWrapper>.Load(CloudKey);

            if (wrapper == null)
            {
                Debug.LogWarning("⚠️ No cloud data found.");
                return false;
            }

            bool valid = PlayerStatsSaveService.ValidateAndImportWrapper(wrapper, db);

            if (valid)
                Debug.Log("✅ Cloud stats loaded and validated.");
            else
                Debug.LogWarning("❌ Cloud stats checksum mismatch.");

            return valid;
        }

        public static async Task DeleteStatsFromCloud()
        {
            await CloudSaveEntity<PlayerStatsSaveWrapper>.Delete(CloudKey);
            Debug.Log("🗑️ Cloud stats deleted.");
        }
    }
}