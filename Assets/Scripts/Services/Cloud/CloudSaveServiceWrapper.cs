using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using UnityEngine;

namespace Services.Cloud
{
    public static  class CloudSaveServiceWrapper
    {
        public static async Task SaveAsync<T>(string key, T data)
        {
            string json = JsonUtility.ToJson(data);
            var dict = new Dictionary<string, object> { { key, json } };
            await CloudSaveService.Instance.Data.ForceSaveAsync(dict);
            Debug.Log($"☁️ Saved key '{key}' to cloud.");
        }

        public static async Task<T> LoadAsync<T>(string key)
        {
            var keys = new HashSet<string> { key };
            var cloudData = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

            if (cloudData.TryGetValue(key, out var item))
            {
                string json = item.Value.GetAs<string>();
                T result = JsonUtility.FromJson<T>(json);
                Debug.Log($"☁️ Loaded key '{key}' from cloud.");
                return result;
            }

            Debug.LogWarning($"⚠️ Key '{key}' not found in cloud.");
            return default;
        }

        public static async Task DeleteAsync(string key)
        {
            await CloudSaveService.Instance.Data.ForceDeleteAsync(key);
            Debug.Log($"🗑️ Deleted key '{key}' from cloud.");
        }
    }
}