using System.Threading.Tasks;
using Services.Cloud;
using UnityEngine;

namespace Services
{
    public class PlayerDataSyncService
    {
        private const string Key_PlayerName = "PlayerName";

        public static async Task<bool> EnsureNicknameSync()
        {
            if (PlayerPrefs.HasKey("PlayerName")) return true;

            string cloudNickname = await CloudNicknameHandler.LoadNickname();
            if (!string.IsNullOrWhiteSpace(cloudNickname))
            {
                PlayerPrefs.SetString("PlayerName", cloudNickname);
                PlayerPrefs.Save();
                Debug.Log($"🔄 Synced nickname from cloud: {cloudNickname}");
                return true;
            }

            return false; // No nickname en ningún lado
        }
    }
}