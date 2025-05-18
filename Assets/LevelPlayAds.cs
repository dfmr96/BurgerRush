using System;
using System.Collections;
using System.Collections.Generic;
using Services.Ads;
using UnityEngine;
using Unity.Services.LevelPlay;
using Mediation = com.unity3d.mediation;


public class LevelPlayAds : MonoBehaviour
{
    private string appKey = "220ee44fd";
    private BannerAdSample _bannerAd;

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
    }

    void OnApplicationPause(bool isPaused) 
    
    { 	 
        IronSource.Agent.onApplicationPause(isPaused);	 
    }

    public void LoadBanner()
    {
        _bannerAd = new BannerAdSample();
        Debug.Log("Load Banner clicked");
    }

    public void DestroyBanner()
    {
        _bannerAd.DestroyBannerAd();
        Debug.Log("Banner destroyed");
    }
}
