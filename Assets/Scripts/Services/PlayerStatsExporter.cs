using Databases;
using Save;
using ScriptableObjects.PlayerData;
using UnityEngine;

namespace Services
{
    public static class PlayerStatsExporter
    {
        public static string ExportToJson(PlayerStatsDatabase db)
        {
            var saveData = new PlayerStatsSaveData();

            foreach (var pair in db.stats)
            {
                var stat = pair.Value;
                var raw = PlayerStatsService.Get(stat);
                saveData.stats.Add(new PlayerStatsSaveData.StatEntry
                {
                    key = stat.statKey,
                    value = raw.ToString()
                });
            }

            return JsonUtility.ToJson(saveData, true);
        }
        
        public static void ImportFromJson(string json, PlayerStatsDatabase db)
        {
            var saveData = JsonUtility.FromJson<PlayerStatsSaveData>(json);

            foreach (var entry in saveData.stats)
            {
                if (!db.stats.ContainsKey(entry.key))
                {
                    Debug.LogWarning($"Stat {entry.key} not found in database. Skipping.");
                    continue;
                }

                var stat = db.stats[entry.key];

                switch (stat.statType)
                {
                    case StatType.Int:
                        if (int.TryParse(entry.value, out int intVal))
                            PlayerPrefs.SetInt(stat.statKey, intVal);
                        break;
                    case StatType.Float:
                        if (float.TryParse(entry.value, out float floatVal))
                            PlayerPrefs.SetFloat(stat.statKey, floatVal);
                        break;
                    case StatType.String:
                        PlayerPrefs.SetString(stat.statKey, entry.value);
                        break;
                    case StatType.Bool:
                        PlayerPrefs.SetInt(stat.statKey, entry.value == "true" ? 1 : 0);
                        break;
                }
            }

            PlayerPrefs.Save();
            Debug.Log("✅ Stats loaded from JSON");
        }
        
        public static void SaveJsonToFile(string path, PlayerStatsDatabase db)
        {
            string json = ExportToJson(db);
            System.IO.File.WriteAllText(path, json);
            Debug.Log($"✅ Stats saved to: {path}");
            //PlayerStatsExporter.SaveJsonToFile(Application.persistentDataPath + "/player_stats.json", statsDB);
        }
    }
}