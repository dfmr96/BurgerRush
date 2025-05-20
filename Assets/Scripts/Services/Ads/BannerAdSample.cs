using Unity.Services.LevelPlay;
using UnityEngine;
using Mediation = com.unity3d.mediation;

namespace Services.Ads
{
    public class BannerAdSample
    {
        private LevelPlayBannerAd bannerAd;
        private bool isLoaded = false;

        public BannerAdSample()
        {
            var size = Mediation.LevelPlayAdSize.CreateAdaptiveAdSize();
            var position = Mediation.LevelPlayBannerPosition.BottomCenter;
            bannerAd = new LevelPlayBannerAd("2ysgx5mehskfdlh3", size, position);

            bannerAd.OnAdLoaded += HandleLoaded;
            bannerAd.OnAdLoadFailed += HandleFailedLoad;

            bannerAd.LoadAd();
        }

        private void HandleFailedLoad(Mediation.LevelPlayAdError obj)
        {
            Debug.LogError("❌ Failed to load banner: " + obj.ErrorMessage);
        }

        private void HandleLoaded(Mediation.LevelPlayAdInfo obj)
        {
            Debug.Log("✅ Banner loaded.");
            isLoaded = true;
        }

        public void Show()
        {
            if (!isLoaded)
            {
                Debug.LogWarning("⚠️ Tried to show banner before it was loaded.");
                return;
            }

            Debug.Log("🎉 Showing banner...");
            bannerAd.ShowAd();
        }

        public void Hide() => bannerAd?.HideAd();

        public void Destroy() => bannerAd?.DestroyAd();

        public bool IsReady() => isLoaded;
    }
}