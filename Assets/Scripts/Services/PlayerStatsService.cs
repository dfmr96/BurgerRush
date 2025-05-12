using Databases;
using ScriptableObjects.PlayerData;
using UnityEngine;

namespace Services
{
    public static class PlayerStatsService
    {
        public static object Get(PlayerStatDefinition stat)
        {
            return stat.statType switch
            {
                StatType.Int => PlayerPrefs.GetInt(stat.statKey, int.Parse(stat.defaultValue)),
                StatType.Float => PlayerPrefs.GetFloat(stat.statKey, float.Parse(stat.defaultValue)),
                StatType.String => PlayerPrefs.GetString(stat.statKey, stat.defaultValue),
                StatType.Bool => PlayerPrefs.GetInt(stat.statKey, stat.defaultValue == "true" ? 1 : 0) == 1,
                _ => null
            };
        }

        public static void Set(PlayerStatDefinition stat, object value)
        {
            switch (stat.statType)
            {
                case StatType.Int: PlayerPrefs.SetInt(stat.statKey, (int)value); break;
                case StatType.Float: PlayerPrefs.SetFloat(stat.statKey, (float)value); break;
                case StatType.String: PlayerPrefs.SetString(stat.statKey, (string)value); break;
                case StatType.Bool: PlayerPrefs.SetInt(stat.statKey, (bool)value ? 1 : 0); break;
            }

            PlayerPrefs.Save();
        }

        public static void Increment(PlayerStatDefinition stat)
        {
            if (stat.statType != StatType.Int) return;

            int current = PlayerPrefs.GetInt(stat.statKey, int.Parse(stat.defaultValue));
            PlayerPrefs.SetInt(stat.statKey, current + 1);
            PlayerPrefs.Save();
        }

        public static void ResetAll(PlayerStatsDatabase db)
        {
            foreach (var stat in db.stats.Values)
                PlayerPrefs.DeleteKey(stat.statKey);

            PlayerPrefs.Save();
        }
        
        public static bool HasAnyStatSaved(PlayerStatsDatabase db)
        {
            foreach (var pair in db.stats)
            {
                string key = pair.Key;
                var stat = pair.Value;

                switch (stat.statType)
                {
                    case StatType.Int:
                        if (PlayerPrefs.HasKey(key) && PlayerPrefs.GetInt(key) != 0)
                            return true;
                        break;
                    case StatType.Float:
                        if (PlayerPrefs.HasKey(key) && PlayerPrefs.GetFloat(key) != 0f)
                            return true;
                        break;
                    case StatType.String:
                        if (PlayerPrefs.HasKey(key) && !string.IsNullOrEmpty(PlayerPrefs.GetString(key)))
                            return true;
                        break;
                    case StatType.Bool:
                        if (PlayerPrefs.HasKey(key) && PlayerPrefs.GetInt(key) != 0)
                            return true;
                        break;
                }
            }

            return false;
        }
        
        public static void InitializeAllStats(PlayerStatsDatabase db)
        {
            foreach (var pair in db.stats)
            {
                var stat = pair.Value;

                if (!PlayerPrefs.HasKey(stat.statKey))
                {
                    switch (stat.statType)
                    {
                        case StatType.Int:
                            PlayerPrefs.SetInt(stat.statKey, 0);
                            break;
                        case StatType.Float:
                            PlayerPrefs.SetFloat(stat.statKey, 0f);
                            break;
                        case StatType.String:
                            PlayerPrefs.SetString(stat.statKey, "");
                            break;
                        case StatType.Bool:
                            PlayerPrefs.SetInt(stat.statKey, 0);
                            break;
                    }
                }
            }

            PlayerPrefs.Save();
        }
        
        
    }
}