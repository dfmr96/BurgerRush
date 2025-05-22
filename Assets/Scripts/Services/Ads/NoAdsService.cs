using System;
using System.Threading.Tasks;
using Services.Cloud;

namespace Services.Ads
{
    public class NoAdsService
    {
        private static bool _hasNoAds;
        public static bool HasNoAds => _hasNoAds;

        public static event Action OnNoAdsUnlocked;

        public static async Task InitializeAsync()
        {
            await UgsInitializer.EnsureInitializedAsync();
            _hasNoAds = await CloudNoAdsHandler.LoadNoAdsStatus();
        }

        public static async Task UnlockNoAdsAsync()
        {
            _hasNoAds = true;
            AdsSettings.SetNoAds(true);
            await AdsSettings.SaveNoAdsToCloud();
            OnNoAdsUnlocked?.Invoke();
        }
    }
}