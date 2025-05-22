using Unity.Services.LevelPlay;
using UnityEngine;
using Mediation = com.unity3d.mediation;

namespace Services.Ads
{
    public class BannerAdSample
    {
        private readonly LevelPlayBannerAd bannerAd;
        private bool isLoaded = false;

        public BannerAdSample()
        {
            var size = Mediation.LevelPlayAdSize.CreateAdaptiveAdSize();
            var position = Mediation.LevelPlayBannerPosition.BottomCenter;
            bannerAd = new LevelPlayBannerAd("2ysgx5mehskfdlh3", size, position);

            bannerAd.OnAdLoaded += _ => {
                Debug.Log("✅ Banner loaded.");
                isLoaded = true;
            };

            bannerAd.OnAdLoadFailed += error =>
                Debug.LogError("❌ Failed to load banner: " + error.ErrorMessage);

            bannerAd.LoadAd();
        }

        public void Show()
        {
            if (!isLoaded)
            {
                Debug.LogWarning("⚠️ Tried to show banner before it was loaded.");
                return;
            }

            Debug.Log("📢 Showing banner...");
            bannerAd.ShowAd();
        }

        public void Hide() => bannerAd?.HideAd();

        public void Destroy() => bannerAd?.DestroyAd();

        public bool IsReady() => isLoaded;
    }
}