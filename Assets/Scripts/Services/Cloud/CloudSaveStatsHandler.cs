using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Databases;
using Unity.Services.CloudSave;
using UnityEngine;

namespace Services.Cloud
{
    public static class CloudSaveStatsHandler
    {
        private const string CloudKey = "PlayerStats";

        public static async Task SaveStatsToCloud(PlayerStatsDatabase db)
        {
            string json = PlayerStatsSaveService.ExportWithChecksum(db);
            var dict = new Dictionary<string, object> { { CloudKey, (object)json } }; // 👈 casteo explícito
            await CloudSaveService.Instance.Data.ForceSaveAsync(dict);
        }

        public static async Task<bool> LoadStatsFromCloud(PlayerStatsDatabase db)
        {
            string json = await CloudSaveServiceWrapper.LoadAsync<string>(CloudKey);

            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning("⚠️ No cloud data found.");
                return false;
            }

            bool valid = PlayerStatsSaveService.ImportWithValidation(json, db);

            if (valid)
                Debug.Log("✅ Cloud stats loaded and validated.");
            else
                Debug.LogWarning("❌ Cloud stats checksum mismatch.");

            return valid;
        }

        public static async Task DeleteStatsFromCloud()
        {
            await CloudSaveServiceWrapper.DeleteAsync(CloudKey);
            Debug.Log("🗑️ Cloud stats deleted.");
        }
    }
}