using System;
using System.Threading.Tasks;
using Services.Cloud;
using UnityEngine;

namespace Services.Ads
{
    public static  class NoAdsService
    {
        private const string LocalKey = "HasNoAds";

        private static bool _hasNoAds;
        public static bool HasNoAds => _hasNoAds;

        public static event Action OnNoAdsUnlocked;
        public static bool IsInitialized { get; private set; }

        public static async Task InitializeAsync()
        {
            if (IsInitialized) return;
            await UgsInitializer.EnsureInitializedAsync();

            // 🔁 Recuperar desde la nube y sincronizar con local
            _hasNoAds = await CloudNoAdsHandler.LoadNoAdsStatus();
            PlayerPrefs.SetInt(LocalKey, _hasNoAds ? 1 : 0);
            PlayerPrefs.Save();

            IsInitialized = true;
            Debug.Log($"🧠 NoAdsService initialized. HasNoAds: {_hasNoAds}");
        }

        public static async Task UnlockNoAdsAsync()
        {
            _hasNoAds = true;

            PlayerPrefs.SetInt(LocalKey, 1);
            PlayerPrefs.Save();

            await CloudNoAdsHandler.SaveNoAdsStatus(true);

            Debug.Log("✅ No Ads unlocked and saved locally and to cloud.");
            OnNoAdsUnlocked?.Invoke();
        }

        // 🔄 Por compatibilidad o casos externos
        public static void SetNoAdsLocalOnly(bool value)
        {
            _hasNoAds = value;
            PlayerPrefs.SetInt(LocalKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }

        public static bool GetNoAdsLocalOnly()
        {
            return PlayerPrefs.GetInt(LocalKey, 0) == 1;
        }
    }
}