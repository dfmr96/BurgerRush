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
        
        //------------------- Checksum Methods ------------------//
        
        [ContextMenu("🔐 Export With Checksum (Editor Field)")]
        public void ExportWithChecksum()
        {
            savedJson = PlayerStatsSaveService.ExportWithChecksum(statsDB);
            Debug.Log("✅ Stats exported with checksum.");
        }

        [ContextMenu("🧪 Validate JSON With Checksum")]
        public void ValidateCurrentJson()
        {
            bool valid = PlayerStatsSaveService.TryImportFromJson(savedJson, statsDB);
            Debug.Log(valid ? "✅ Checksum is valid." : "❌ Checksum mismatch detected.");
        }

        [ContextMenu("💾 Save Checksum JSON to File")]
        public void SaveChecksumJsonToFile()
        {
            string json = PlayerStatsSaveService.ExportWithChecksum(statsDB);
            File.WriteAllText(FilePath, json);
            Debug.Log($"💾 Checksum save written to {FilePath}");
        }
        
        [ContextMenu("🔄 Restore From File With Checksum Validation")]
        public void RestoreChecksumJsonFromFile()
        {
            if (!File.Exists(FilePath))
            {
                Debug.LogWarning("⚠ Backup file with checksum not found.");
                return;
            }

            string json = File.ReadAllText(FilePath);
            bool valid = PlayerStatsSaveService.TryImportFromJson(json, statsDB);
            Debug.Log(valid ? "✅ Restored from file with valid checksum." : "❌ Checksum mismatch. Restore aborted.");
        }
    }
}