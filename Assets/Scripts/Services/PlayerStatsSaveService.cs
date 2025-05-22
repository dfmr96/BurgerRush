using System;
using Databases;
using Save;
using Services.Utils;
using UnityEngine;

namespace Services
{
    public static class PlayerStatsSaveService
    {
        // ─────────────────────────────────────────────────────────────
        // 📦 Constantes
        // ─────────────────────────────────────────────────────────────

        private const string SavedOnceKey = "HasSavedOnce";
        private const string TotalPlayTimeKey = "TotalSecondsPlayed";

        // ─────────────────────────────────────────────────────────────
        // 🧩 Exportación
        // ─────────────────────────────────────────────────────────────

        public static string ExportWithChecksum(PlayerStatsDatabase db)
        {
            var wrapper = CreateWrapperWithChecksum(db);
            string json = JsonUtility.ToJson(wrapper, true);
            Debug.Log($"📦 Exported JSON with checksum:\n{json}");
            return json;
        }

        public static PlayerStatsSaveWrapper CreateWrapperWithChecksum(PlayerStatsDatabase db)
        {
            var saveData = new PlayerStatsSaveData();

            foreach (var pair in db.stats)
            {
                var value = PlayerStatsService.Get(pair.Value);
                saveData.stats.Add(new PlayerStatsSaveData.StatEntry
                {
                    key = pair.Key,
                    value = value.ToString()
                });
            }

            var wrapper = new PlayerStatsSaveWrapper
            {
                data = saveData,
                lastSavedAt = DateTime.UtcNow.Ticks,
                checksum = GenerateChecksum(saveData)
            };

            return wrapper;
        }

        // ─────────────────────────────────────────────────────────────
        // 📥 Importación
        // ─────────────────────────────────────────────────────────────

        public static bool TryImportFromJson(string json, PlayerStatsDatabase db)
        {
            if (!TryParseWrapper(json, out var wrapper)) return false;
            return ValidateAndApplyWrapper(wrapper, db);
        }

        public static bool ValidateAndApplyWrapper(PlayerStatsSaveWrapper wrapper, PlayerStatsDatabase db)
        {
            if (!ValidateChecksum(wrapper))
            {
                Debug.LogWarning("❌ Checksum mismatch. Import aborted.");
                return false;
            }

            string cleanJson = JsonUtility.ToJson(wrapper.data);
            PlayerStatsExporter.ImportFromJson(cleanJson, db);
            return true;
        }

        private static bool TryParseWrapper(string json, out PlayerStatsSaveWrapper wrapper)
        {
            wrapper = null;
            if (string.IsNullOrWhiteSpace(json)) return false;

            try
            {
                wrapper = JsonUtility.FromJson<PlayerStatsSaveWrapper>(json);
                return wrapper?.data != null;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"⚠️ Failed to parse wrapper: {e.Message}");
                return false;
            }
        }

        // ─────────────────────────────────────────────────────────────
        // ✅ Checksum
        // ─────────────────────────────────────────────────────────────

        public static string GenerateChecksum(PlayerStatsSaveData data)
        {
            string json = JsonUtility.ToJson(data);
            return SaveCheckSumUtility.GenerateChecksum(json);
        }

        public static bool ValidateChecksum(PlayerStatsSaveWrapper wrapper)
        {
            if (wrapper == null || wrapper.data == null) return false;

            string expected = GenerateChecksum(wrapper.data);
            bool match = expected == wrapper.checksum;

            if (!match)
            {
                Debug.LogWarning($"❌ Invalid checksum. Expected: {expected}, Found: {wrapper.checksum}");
            }

            return match;
        }

        public static bool AreChecksumsEqual(string jsonA, string jsonB)
        {
            string a = ExtractChecksum(jsonA);
            string b = ExtractChecksum(jsonB);

            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
            {
                Debug.LogWarning("⚠️ One or both JSONs lack a checksum.");
                return false;
            }

            return a == b;
        }

        private static string ExtractChecksum(string json)
        {
            if (TryParseWrapper(json, out var wrapper))
            {
                return wrapper.checksum;
            }

            return null;
        }

        // ─────────────────────────────────────────────────────────────
        // 🔄 Estado de instalación
        // ─────────────────────────────────────────────────────────────

        public static bool IsFreshInstall() =>
            PlayerPrefs.GetInt(SavedOnceKey, 0) == 0;

        public static void MarkAsSaved()
        {
            PlayerPrefs.SetInt(SavedOnceKey, 1);
            PlayerPrefs.Save();
        }

        // ─────────────────────────────────────────────────────────────
        // ⏱ Tiempo total jugado
        // ─────────────────────────────────────────────────────────────

        public static int GetTotalPlayTimeFromWrapper(PlayerStatsSaveWrapper wrapper)
        {
            if (wrapper?.data?.stats == null) return 0;

            foreach (var entry in wrapper.data.stats)
            {
                if (entry.key == TotalPlayTimeKey && int.TryParse(entry.value, out int seconds))
                {
                    return seconds;
                }
            }

            return 0;
        }
    }
}
