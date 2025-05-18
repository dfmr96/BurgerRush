using Unity.Services.LevelPlay;
using UnityEngine;
using Mediation = com.unity3d.mediation;

namespace Services.Ads
{
    public class BannerAdSample
    {
        private LevelPlayBannerAd bannerAd;

        public BannerAdSample()
        {
            var size = Mediation.LevelPlayAdSize.CreateAdaptiveAdSize();
            var position = Mediation.LevelPlayBannerPosition.BottomCenter;
            bannerAd = new LevelPlayBannerAd("2ysgx5mehskfdlh3", size, position);

            bannerAd.LoadAd();
        }

        public void Show() => bannerAd?.ShowAd();

        public void Hide() => bannerAd?.HideAd();

        public void Destroy() => bannerAd?.DestroyAd();
    }
}