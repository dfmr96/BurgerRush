using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using UnityEngine;

namespace Services.Cloud
{
    public static class CloudSaveEntity<T>
    {
        public static async Task Save(string key, T data)
        {
            string json = JsonUtility.ToJson(data);
            var dict = new Dictionary<string, object> { { key, json } };
            await CloudSaveService.Instance.Data.ForceSaveAsync(dict);
            Debug.Log($"☁️ Saved <{typeof(T).Name}> to cloud under key '{key}'.");
        }

        public static async Task<T> Load(string key)
        {
            var keys = new HashSet<string> { key };
            var cloudData = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

            if (cloudData.TryGetValue(key, out var item))
            {
                string json = item.Value.GetAs<string>();
                
                if (typeof(T) == typeof(string))
                {
                    Debug.Log($"☁️ Loaded <string> from cloud key '{key}'.");
                    return (T)(object)json;
                }
                
                T result = JsonUtility.FromJson<T>(json);
                Debug.Log($"☁️ Loaded <{typeof(T).Name}> from cloud key '{key}'.");
                return result;
            }

            Debug.LogWarning($"⚠️ Cloud key '{key}' not found.");
            return default;
        }

        public static async Task Delete(string key)
        {
            await CloudSaveService.Instance.Data.ForceDeleteAsync(key);
            Debug.Log($"🗑️ Deleted cloud key '{key}'.");
        }
    }
}