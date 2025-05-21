using System;
using Databases;
using Save;
using Services.Utils;
using UnityEngine;

namespace Services
{
    public static class PlayerStatsSaveService
    {
        public static string ExportWithChecksum(PlayerStatsDatabase db)
        {
            var wrapper = ExportWrapperWithChecksum(db);
            string finalJson = JsonUtility.ToJson(wrapper, true);
            Debug.Log($"📦 Final JSON with checksum:\n{finalJson}");
            return finalJson;
        }

        public static bool ImportWithValidation(string json, PlayerStatsDatabase db)
        {
            var wrapper = JsonUtility.FromJson<PlayerStatsSaveWrapper>(json);
            if (!ValidateChecksum(wrapper))
            {
                Debug.LogWarning("❌ Checksum mismatch: data may be corrupted or tampered.");
                return false;
            }

            PlayerStatsExporter.ImportFromJson(JsonUtility.ToJson(wrapper.data), db);
            return true;
        }

        public static PlayerStatsSaveWrapper ExportWrapperWithChecksum(PlayerStatsDatabase db)
        {
            var rawData = new PlayerStatsSaveData();

            foreach (var pair in db.stats)
            {
                var value = PlayerStatsService.Get(pair.Value);
                rawData.stats.Add(new PlayerStatsSaveData.StatEntry
                {
                    key = pair.Key,
                    value = value.ToString()
                });
            }

            var wrapper = new PlayerStatsSaveWrapper
            {
                data = rawData,
                lastSavedAt = DateTime.UtcNow.Ticks
            };

            wrapper.checksum = GenerateChecksum(wrapper);

            // ❌ NO marcar como "has saved" acá.
            return wrapper;
        }

        public static string GenerateChecksum(PlayerStatsSaveWrapper wrapper)
        {
            // ✅ Solo se incluye `data`, no `lastSavedAt`
            string json = JsonUtility.ToJson(wrapper.data);
            return SaveCheckSumUtility.GenerateChecksum(json);
        }

        public static bool ValidateAndImportWrapper(PlayerStatsSaveWrapper wrapper, PlayerStatsDatabase db)
        {
            if (!ValidateChecksum(wrapper))
            {
                Debug.LogWarning("❌ Checksum mismatch: data may be corrupted or tampered.");
                return false;
            }

            PlayerStatsExporter.ImportFromJson(JsonUtility.ToJson(wrapper.data), db);
            return true;
        }

        public static bool ValidateChecksum(PlayerStatsSaveWrapper wrapper)
        {
            if (wrapper == null || wrapper.data == null)
                return false;

            string recomputed = GenerateChecksum(wrapper);
            return wrapper.checksum == recomputed;
        }

        public static bool AreChecksumsEqual(string jsonA, string jsonB)
        {
            string checksumA = ExtractChecksum(jsonA);
            string checksumB = ExtractChecksum(jsonB);

            if (string.IsNullOrEmpty(checksumA) || string.IsNullOrEmpty(checksumB))
            {
                Debug.LogWarning("⚠️ One or both JSONs lack a valid checksum.");
                return false;
            }

            return checksumA == checksumB;
        }

        private static string ExtractChecksum(string json)
        {
            try
            {
                var wrapper = JsonUtility.FromJson<PlayerStatsSaveWrapper>(json);
                return wrapper.checksum;
            }
            catch
            {
                Debug.LogWarning("⚠️ Could not extract checksum from JSON.");
                return null;
            }
        }

        public static bool IsFreshInstall()
        {
            return PlayerPrefs.GetInt("HasSavedOnce", 0) == 0;
        }
        
        public static void MarkAsSaved()
        {
            PlayerPrefs.SetInt("HasSavedOnce", 1);
            PlayerPrefs.Save();
        }
        
        public static int GetTotalPlayTimeFromWrapper(PlayerStatsSaveWrapper wrapper)
        {
            foreach (var entry in wrapper.data.stats)
            {
                if (entry.key == "TotalSecondsPlayed")
                {
                    int value = Convert.ToInt32(entry.value);
                    return value;
                }
            }
            return 0;
        }
    }
}
