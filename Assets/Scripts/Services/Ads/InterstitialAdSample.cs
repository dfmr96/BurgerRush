using Unity.Services.LevelPlay;
using UnityEngine.UI;

namespace Services.Ads
{
    public class InterstitialAdSample
    {
        private LevelPlayInterstitialAd interstitialAd;
        private Button loadInterstitial;

        public InterstitialAdSample()
        {
            //Create InterstitialAd instance
            CreateInterstitialAd();
        }

        void CreateInterstitialAd()
        {
            //Create InterstitialAd instance
            interstitialAd = new LevelPlayInterstitialAd("0ot85tyr3a7uy2qe");
            //Subscribe InterstitialAd events
            interstitialAd.OnAdLoaded += InterstitialOnAdLoadedEvent;
            interstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailedEvent;
            interstitialAd.OnAdDisplayed += InterstitialOnAdDisplayedEvent;
            interstitialAd.OnAdDisplayFailed += InterstitialOnAdDisplayFailedEvent;
            interstitialAd.OnAdClicked += InterstitialOnAdClickedEvent;
            interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
            interstitialAd.OnAdInfoChanged += InterstitialOnAdInfoChangedEvent;
            
            //LoadInterstitialAd();
        }

        public void LoadInterstitialAd(Button showButton)
        {
            //Load or reload InterstitialAd 	
            interstitialAd.LoadAd();
            loadInterstitial = showButton;
        }

        public void ShowInterstitialAd()
        {
            //Show InterstitialAd, check if the ad is ready before showing
            if (interstitialAd.IsAdReady())
            {
                interstitialAd.ShowAd();
            }
        }

        void DestroyInterstitialAd()
        {
            //Destroy InterstitialAd 
            interstitialAd.DestroyAd();
        }

        //Implement InterstitialAd events
        void InterstitialOnAdLoadedEvent(LevelPlayAdInfo adInfo)
        {
            loadInterstitial.interactable = true;
        }

        void InterstitialOnAdLoadFailedEvent(LevelPlayAdError ironSourceError)
        {
        }

        void InterstitialOnAdClickedEvent(LevelPlayAdInfo adInfo)
        {
        }

        void InterstitialOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
        {
        }

        void InterstitialOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError adInfoError)
        {
        }

        void InterstitialOnAdClosedEvent(LevelPlayAdInfo adInfo)
        {
        }

        void InterstitialOnAdInfoChangedEvent(LevelPlayAdInfo adInfo)
        {
        }
    }
}