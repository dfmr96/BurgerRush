using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.unity3d.mediation;

public class LevelPlayAds : MonoBehaviour
{
    private string adUnitId = "2ysgx5mehskfdlh3";
    private LevelPlayBannerAd bannerAd;

    private void Start()
    {
// Init the SDK when implementing the Multiple Ad Units API for Interstitial and Banner formats, with Rewarded using legacy APIs 
        //LevelPlayAdFormat[] legacyAdFormats = new[] { LevelPlayAdFormat.REWARDED, LevelPlayAdFormat.BANNER, LevelPlayAdFormat.INTERSTITIAL};
        IronSource.Agent.setMetaData("is_test_suite", "enable");
        IronSource.Agent.validateIntegration();
        
        //LevelPlay.Init("220ee44fd");
        IronSource.Agent.init("220ee44fd");
    }

    private void OnEnable()
    {
        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
        LevelPlay.OnInitFailed += SdkInitializationFailedEvent;
        IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;

// Implement the events

    }

    private void SdkInitializationCompletedEvent()
    {
        IronSource.Agent.launchTestSuite();
    }

    private void SdkInitializationFailedEvent(LevelPlayInitError obj)
    {
        //throw new NotImplementedException();
    }

    private void SdkInitializationCompletedEvent(LevelPlayConfiguration obj)
    {
        Debug.Log("LevelPlay SDK initialized successfully. Calling Test Suite");
        IronSource.Agent.launchTestSuite();
    }

    void OnApplicationPause(bool isPaused) 
    
    { 	 
        IronSource.Agent.onApplicationPause(isPaused);	 
    }

    public void LoadBanner()
    {
        bannerAd = new LevelPlayBannerAd(adUnitId, LevelPlayAdSize.BANNER
            , LevelPlayBannerPosition.BottomCenter);
        // Register to the events 
        bannerAd.OnAdLoaded += BannerOnAdLoadedEvent;
        bannerAd.OnAdLoadFailed += BannerOnAdLoadFailedEvent;
        bannerAd.OnAdDisplayed += BannerOnAdDisplayedEvent;
        bannerAd.OnAdDisplayFailed += BannerOnAdDisplayFailedEvent;
        bannerAd.OnAdClicked += BannerOnAdClickedEvent;
        bannerAd.OnAdCollapsed += BannerOnAdCollapsedEvent;
        bannerAd.OnAdLeftApplication += BannerOnAdLeftApplicationEvent;
        bannerAd.OnAdExpanded += BannerOnAdExpandedEvent;
    }

    public void DestroyBanner()
    {
        bannerAd.DestroyAd();
    }
    // BANNER EVENTS
       private void BannerOnAdLoadedEvent(LevelPlayAdInfo adInfo) {}
       private void BannerOnAdLoadFailedEvent(LevelPlayAdError ironSourceError) {}
       private void BannerOnAdClickedEvent(LevelPlayAdInfo adInfo) {}
       private void BannerOnAdDisplayedEvent(LevelPlayAdInfo adInfo) {}
       private void BannerOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError adInfoError) {}
       private void BannerOnAdCollapsedEvent(LevelPlayAdInfo adInfo) {}
       private void BannerOnAdLeftApplicationEvent(LevelPlayAdInfo adInfo) {}
       private void BannerOnAdExpandedEvent(LevelPlayAdInfo adInfo) {}
}
