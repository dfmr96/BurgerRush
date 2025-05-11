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
    }
}