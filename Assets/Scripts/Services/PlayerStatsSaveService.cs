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
    
            foreach (var pair in db.stats)
            {
                var value = PlayerStatsService.Get(pair.Value);
                rawData.stats.Add(new PlayerStatsSaveData.StatEntry
                {
                    key = pair.Key,
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

            return JsonUtility.ToJson(wrapper, true);
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