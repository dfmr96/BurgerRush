using System.IO;
using Databases;
using ScriptableObjects.PlayerData;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class PlayerStatsGenerator
    {
        private static readonly (string key, string displayName)[] Stats = new (string, string)[]
    {
        ("TotalPlays", "Total Plays"),
        ("TotalBurgersDelivered", "Burgers Delivered"),
        ("TotalOrdersFailed", "Orders Failed"),
        ("TotalBurgersDiscarded", "Burgers Discarded"),
        ("TotalIngredientsPlaced", "Ingredients Placed"),
        ("TotalSecondsPlayed", "Seconds Played"),
        ("OrdersOnTime", "Orders On Time"),
        ("ClutchDeliveries", "Clutch Deliveries"),
        ("PerfectBurgerStreak", "Perfect Streak"),
        ("HighScore", "High Score"),
        ("MaxStreak", "Max Streak"),
        ("PlayerName", "Player Name"),
        ("DifficultyUnlocked", "Difficulty Unlocked"),
        ("EasyDeliveries", "Easy Deliveries"),
        ("MediumDeliveries", "Medium Deliveries"),
        ("HardDeliveries", "Hard Deliveries"),
        ("EasyFails", "Easy Fails"),
        ("MediumFails", "Medium Fails"),
        ("HardFails", "Hard Fails"),
        ("LongestSessionDuration", "Longest Session")
    };

    [MenuItem("Tools/Generate Player Stats Database")]
    public static void GenerateStats()
    {
        string basePath = "Assets/Scripts/ScriptableObjects/PlayerData";
        string dbPath = $"{basePath}/PlayerStatsDatabase.asset";

        if (!AssetDatabase.IsValidFolder(basePath))
            Directory.CreateDirectory(basePath);

        var database = ScriptableObject.CreateInstance<PlayerStatsDatabase>();
        database.stats = new PlayerStatsDatabase.StatDict();

        foreach (var (key, name) in Stats)
        {
            var asset = ScriptableObject.CreateInstance<PlayerStatDefinition>();
            asset.statKey = key;
            asset.displayName = name;

            if (key == "PlayerName")
            {
                asset.statType = StatType.String;
                asset.defaultValue = "Player";
            }
            else
            {
                asset.statType = StatType.Int;
                asset.defaultValue = "0";
            }

            string assetPath = $"{basePath}/{key}.asset";
            AssetDatabase.CreateAsset(asset, assetPath);
            database.stats.Add(key, asset);
        }

        AssetDatabase.CreateAsset(database, dbPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("✅ PlayerStatsDatabase generated with all default stats.");
    }
    }
}