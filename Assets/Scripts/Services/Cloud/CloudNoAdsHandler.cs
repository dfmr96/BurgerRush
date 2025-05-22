using System.Threading.Tasks;
using Save;
using UnityEngine;

namespace Services.Cloud
{
    public static class CloudNoAdsHandler
    {
        private const string CloudKey = "NoAdsStatus";

        public static async Task SaveNoAdsStatus(bool hasNoAds)
        {
            await UgsInitializer.EnsureInitializedAsync(); // 🔐 protección
            var data = new NoAdsStatusData { hasNoAds = hasNoAds };
            await CloudSaveEntity<NoAdsStatusData>.Save(CloudKey, data);
        }

        public static async Task<bool> LoadNoAdsStatus()
        {
            await UgsInitializer.EnsureInitializedAsync();

            var data = await CloudSaveEntity<NoAdsStatusData>.Load(CloudKey);

            if (data == null)
            {
                Debug.LogWarning("⚠️ NoAdsStatus data is null — returning false by default.");
                return false;
            }

            return data.hasNoAds;
        }

        public static async Task DeleteNoAdsStatus()
        {
            await UgsInitializer.EnsureInitializedAsync(); // 🔐 protección
            await CloudSaveEntity<NoAdsStatusData>.Delete(CloudKey);
        }
    }
}