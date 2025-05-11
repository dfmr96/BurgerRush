using Databases;
using ScriptableObjects.PlayerData;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class PlayerStatsDatabasePopulator
    {
        [MenuItem("Tools/Populate PlayerStatsDatabase")]
        public static void PopulateDatabase()
        {
            string dbPath = "Assets/Scripts/Databases/PlayerStatsDatabase.asset";
            var db = AssetDatabase.LoadAssetAtPath<PlayerStatsDatabase>(dbPath);

            if (db == null)
            {
                Debug.LogError("❌ PlayerStatsDatabase.asset not found at: " + dbPath);
                return;
            }

            db.stats.Clear();

            string[] guids = AssetDatabase.FindAssets("t:PlayerStatDefinition", new[] { "Assets/Scripts/ScriptableObjects/PlayerData" });

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var stat = AssetDatabase.LoadAssetAtPath<PlayerStatDefinition>(path);

                if (stat != null && !string.IsNullOrEmpty(stat.statKey))
                {
                    if (!db.stats.ContainsKey(stat.statKey))
                        db.stats.Add(stat.statKey, stat);
                    else
                        Debug.LogWarning($"⚠️ Duplicate statKey found: {stat.statKey}, skipping.");
                }
            }

            EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"✅ PlayerStatsDatabase populated with {db.stats.Count} stats.");
        }
    }
}