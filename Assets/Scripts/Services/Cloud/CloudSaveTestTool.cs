using Databases;
using UnityEngine;

namespace Services.Cloud
{
    public class CloudSaveTestTool : MonoBehaviour

    {
    [SerializeField] private PlayerStatsDatabase statsDB;

    [ContextMenu("☁️ Save Stats to Cloud")]
    public async void SaveToCloud()
    {
        await CloudSaveStatsHandler.SaveStatsToCloud(statsDB);
    }

    [ContextMenu("☁️ Load Stats from Cloud")]
    public async void LoadFromCloud()
    {
        await CloudSaveStatsHandler.LoadStatsFromCloud(statsDB);
    }

    [ContextMenu("🗑️ Delete Stats from Cloud")]
    public async void DeleteCloudStats()
    {
        await CloudSaveStatsHandler.DeleteStatsFromCloud();
    }
    }
}