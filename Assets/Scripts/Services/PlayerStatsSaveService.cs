using Databases;
using Save;
using Services.Utils;
using UnityEngine;

namespace Services
{
    public static  class PlayerStatsSaveService
    {
        public static string ExportWithChecksum(PlayerStatsDatabase db)
        {
            var rawData = new PlayerStatsSaveData();
            
            Debug.Log("📊 Exporting player stats with checksum...");
    
            foreach (var pair in db.stats)
            {
                var stat = pair.Value;
                var value = PlayerStatsService.Get(stat);

                // ✅ Log para verificar que se están guardando datos reales
                Debug.Log($"✔️ {stat.statKey} = {value}");

                rawData.stats.Add(new PlayerStatsSaveData.StatEntry
                {
                    key = stat.statKey,
                    value = value.ToString()
                });
            }

            string jsonData = JsonUtility.ToJson(rawData);
            string checksum = SaveCheckSumUtility.GenerateChecksum(jsonData);

            var wrapper = new PlayerStatsSaveWrapper
            {
                data = rawData,
                checksum = checksum
            };

            string finalJson = JsonUtility.ToJson(wrapper, true);
            Debug.Log($"📦 Final JSON with checksum:\n{finalJson}");

            return finalJson;
        }
        
        public static bool ImportWithValidation(string json, PlayerStatsDatabase db)
        {
            var wrapper = JsonUtility.FromJson<PlayerStatsSaveWrapper>(json);
            string recomputed = SaveCheckSumUtility.GenerateChecksum(JsonUtility.ToJson(wrapper.data));

            if (wrapper.checksum != recomputed)
            {
                Debug.LogWarning("❌ Checksum mismatch: data may be corrupted or tampered.");
                return false;
            }

            PlayerStatsExporter.ImportFromJson(JsonUtility.ToJson(wrapper.data), db);
            return true;
        }
    }
}