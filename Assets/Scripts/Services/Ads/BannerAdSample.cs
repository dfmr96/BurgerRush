using System;
using Unity.Services.LevelPlay;
using UnityEngine;

namespace Services.Ads
{
    public class BannerAdSample
    {
        private LevelPlayBannerAd bannerAd;

        public BannerAdSample()
        {
            CreateBannerAd();
        }

        private void CreateBannerAd()
        {
            //Create banner instance
            var size = com.unity3d.mediation.LevelPlayAdSize.CreateAdaptiveAdSize();
            int width = size.Width;
            int height = size.Height;
            var position = com.unity3d.mediation.LevelPlayBannerPosition.BottomCenter;

            bannerAd = new LevelPlayBannerAd("2ysgx5mehskfdlh3", size, position);
            
            Debug.Log("Banner created");
            //Subscribe BannerAd events
            bannerAd.OnAdLoaded += BannerOnAdLoadedEvent;
            bannerAd.OnAdLoadFailed += BannerOnAdLoadFailedEvent;
            bannerAd.OnAdDisplayed += BannerOnAdDisplayedEvent;
            bannerAd.OnAdDisplayFailed += BannerOnAdDisplayFailedEvent;
            bannerAd.OnAdClicked += BannerOnAdClickedEvent;
            bannerAd.OnAdCollapsed += BannerOnAdCollapsedEvent;
            bannerAd.OnAdLeftApplication += BannerOnAdLeftApplicationEvent;
            bannerAd.OnAdExpanded += BannerOnAdExpandedEvent;
            Debug.Log("Events subscribed");
            //Load the banner ad
            LoadBannerAd();
        }

        private void LoadBannerAd()
        {
            //Load the banner ad 
            bannerAd.LoadAd();
        }

        void ShowBannerAd()
        {
            //Show the banner ad, call this method only if you turned off the auto show when you created this banner instance.
            bannerAd.ShowAd();
        }

        void HideBannerAd()
        {
            //Hide banner
            bannerAd.HideAd();
        }

        public void DestroyBannerAd()
        {
            //Destroy banner
            bannerAd.DestroyAd();
        }

        void BannerOnAdLoadedEvent(LevelPlayAdInfo adInfo)
        {
            Debug.Log("Banner loaded successfully");
        }

        void BannerOnAdLoadFailedEvent(LevelPlayAdError ironSourceError)
        {
            Debug.Log("Banner load failed: " + ironSourceError);
        }

        void BannerOnAdClickedEvent(LevelPlayAdInfo adInfo)
        {
            Debug.Log("Banner clicked");
        }

        void BannerOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
        {
            Debug.Log("Banner displayed");
        }

        void BannerOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError adInfoError)
        {
            Debug.Log("Banner display failed: " + adInfoError);
        }


        void BannerOnAdCollapsedEvent(LevelPlayAdInfo adInfo)
        {
            Debug.Log("Banner collapsed");
        }

        void BannerOnAdLeftApplicationEvent(LevelPlayAdInfo adInfo)
        {
            Debug.Log("Banner left application");
        }

        void BannerOnAdExpandedEvent(LevelPlayAdInfo adInfo)
        {
            Debug.Log("Banner expanded");
        }
    }
}