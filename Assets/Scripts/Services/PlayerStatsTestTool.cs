using System.IO;
using Databases;
using UnityEngine;

namespace Services
{
    public class PlayerStatsTestTool : MonoBehaviour
    {
        [SerializeField] private PlayerStatsDatabase statsDB;
        [TextArea(10, 20)] public string savedJson;
        private string backupJson;
        private string FilePath => Path.Combine(Application.persistentDataPath, "player_stats_backup.json");

        [ContextMenu("⬆ Export To JSON (Editor Field)")]
        public void ExportToJSON()
        {
            savedJson = PlayerStatsExporter.ExportToJson(statsDB);
            Debug.Log("✅ Stats exported to JSON string.");
        }

        [ContextMenu("🗑 Backup + Reset Stats (Save File)")]
        public void BackupAndResetStats()
        {
            backupJson = PlayerStatsExporter.ExportToJson(statsDB);
            File.WriteAllText(FilePath, backupJson);
            PlayerStatsService.ResetAll(statsDB);
            Debug.Log($"🗑 All stats reset. Backup saved at: {FilePath}");
        }

        [ContextMenu("♻ Restore From Backup File")]
        public void RestoreFromBackup()
        {
            if (!File.Exists(FilePath))
            {
                Debug.LogWarning("⚠ No backup file found.");
                return;
            }

            string json = File.ReadAllText(FilePath);
            PlayerStatsExporter.ImportFromJson(json, statsDB);
            Debug.Log("✅ Stats restored from backup file.");
        }

        [ContextMenu("⬇ Import From Editor Field")]
        public void ImportFromSavedJsonField()
        {
            PlayerStatsExporter.ImportFromJson(savedJson, statsDB);
        }
    }
}