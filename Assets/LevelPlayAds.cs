using System;
using System.Collections;
using System.Collections.Generic;
using Services.Ads;
using UnityEngine;
using Unity.Services.LevelPlay;
using UnityEngine.UI;
using Mediation = com.unity3d.mediation;


public class LevelPlayAds : MonoBehaviour
{
    private string appKey = "220ee44fd";
    private BannerAdSample _bannerAd;
    private InterstitialAdSample _interstitialAd;
    private RewardedAdSample _rewardedAd;

    [SerializeField] private Button loadInterstitial;
    [SerializeField] private Button showInterstitial;
    [SerializeField] private Button loadRewarded;
    [SerializeField] private Button showRewarded;

    private void Start()
    {
        //IronSource.Agent.init(appKey);
        LevelPlay.Init(appKey);
    }

    private void OnEnable()
    {
        //IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
        LevelPlay.OnInitFailed += SdkInitializationFailedEvent;
    }

    private void SdkInitializationCompletedEvent()
    {
        Debug.Log("SDK Init completed");
    }

    private void SdkInitializationFailedEvent(LevelPlayInitError obj)
    {
        Debug.Log("SDK Init failed");
    }

    private void SdkInitializationCompletedEvent(LevelPlayConfiguration obj)
    {
        Debug.Log("SDK Init completed");
        LoadBanner();
        Debug.Log("Load Banner called");

        loadInterstitial.interactable = true;
        loadRewarded.interactable = true;

        _interstitialAd = new InterstitialAdSample();
        _rewardedAd = new RewardedAdSample();
        
    }

    void OnApplicationPause(bool isPaused) 
    
    { 	 
        IronSource.Agent.onApplicationPause(isPaused);	 
    }

    public void LoadBanner()
    {
        _bannerAd = new BannerAdSample();
        Debug.Log("Load Banner being loaded");
    }

    public void DestroyBanner()
    {
        _bannerAd.DestroyBannerAd();
        Debug.Log("Banner destroyed");
    }

    public void LoadInterstitial()
    {
        _interstitialAd.LoadInterstitialAd(showInterstitial);
        Debug.Log("Load Interstitial clicked");
    }
    
    public void ShowInterstitial()
    {
        _interstitialAd.ShowInterstitialAd();
        Debug.Log("Show Interstitial clicked");
    }

    public void ShowRewarded()
    {
        _rewardedAd.ShowRewardedAd();
        Debug.Log("Show Rewarded clicked");
    }
    
    public void LoadRewarded()
    {
        _rewardedAd.LoadRewardedAd(showRewarded);
        Debug.Log("Load Rewarded clicked");
    }
}
