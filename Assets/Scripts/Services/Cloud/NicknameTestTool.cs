using UnityEngine;

namespace Services.Cloud
{
    public class NicknameTestTool : MonoBehaviour
    {
        [Header("Nickname Test")]
        [SerializeField] private string nicknameToSave;

        [ContextMenu("💾 Save Nickname to Cloud")]
        public async void SaveNickname()
        {
            await CloudNicknameHandler.SaveNickname(nicknameToSave);
            Debug.Log($"✅ Nickname '{nicknameToSave}' saved to cloud.");
        }

        [ContextMenu("☁️ Load Nickname from Cloud")]
        public async void LoadNickname()
        {
            string nickname = await CloudNicknameHandler.LoadNickname();
            Debug.Log($"☁️ Loaded nickname from cloud: {nickname}");
        }

        [ContextMenu("🗑 Delete Nickname from Cloud")]
        public async void DeleteNickname()
        {
            await CloudNicknameHandler.DeleteNickname();
            Debug.Log("🗑 Nickname deleted from cloud.");
        }
    }
}