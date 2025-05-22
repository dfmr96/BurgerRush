using Unity.Services.LevelPlay;
using UnityEngine;
using System;

namespace Services.Ads
{
    public class InterstitialAdSample
    {
        private readonly LevelPlayInterstitialAd ad;
        private Action onClosedCallback;

        public InterstitialAdSample()
        {
            ad = new LevelPlayInterstitialAd("0ot85tyr3a7uy2qe");

            ad.OnAdClosed += HandleAdClosed;
            ad.OnAdLoaded += _ => Debug.Log("✅ Interstitial Loaded.");
            ad.OnAdLoadFailed += _ => Debug.LogWarning("❌ Interstitial Load Failed.");

            Load();
        }

        private void HandleAdClosed(LevelPlayAdInfo info)
        {
            Debug.Log("🛑 Interstitial closed.");
            onClosedCallback?.Invoke();
            onClosedCallback = null; // cleanup
        }

        public void Load() => ad.LoadAd();

        public bool IsReady() => ad.IsAdReady();

        public void Show(Action onClosed)
        {
            if (!IsReady())
            {
                Debug.LogWarning("⚠️ Interstitial tried to show while not ready.");
                return;
            }

            onClosedCallback = onClosed;
            ad.ShowAd();
        }
    }
}