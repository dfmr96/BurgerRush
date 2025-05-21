using System.Threading.Tasks;
using UnityEngine;

namespace Services.Ads
{
    public static class AdsSettings
    {
        private const string NoAdsKey = "HasNoAds";

        public static bool HasNoAds()
        {
            return PlayerPrefs.GetInt(NoAdsKey, 0) == 1;
        }

        public static void SetNoAds(bool value)
        {
            PlayerPrefs.SetInt(NoAdsKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }

        public static async Task SaveNoAdsToCloud()
        {
            await Cloud.CloudNoAdsHandler.SaveNoAdsStatus(HasNoAds());
        }

        public static async Task LoadNoAdsFromCloud()
        {
            bool cloudValue = await Cloud.CloudNoAdsHandler.LoadNoAdsStatus();
            SetNoAds(cloudValue);
        }
    }
}