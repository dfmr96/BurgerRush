using System.Threading.Tasks;
using Save;

namespace Services.Cloud
{
    public static class CloudNoAdsHandler
    {
        private const string CloudKey = "NoAdsStatus";

        public static async Task SaveNoAdsStatus(bool hasNoAds)
        {
            var data = new NoAdsStatusData { hasNoAds = hasNoAds };
            await CloudSaveEntity<NoAdsStatusData>.Save(CloudKey, data);
        }

        public static async Task<bool> LoadNoAdsStatus()
        {
            var data = await CloudSaveEntity<NoAdsStatusData>.Load(CloudKey);
            return data.hasNoAds;
        }

        public static async Task DeleteNoAdsStatus()
        {
            await CloudSaveEntity<NoAdsStatusData>.Delete(CloudKey);
        }
    }
}