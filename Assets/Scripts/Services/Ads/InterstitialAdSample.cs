using Unity.Services.LevelPlay;
using UnityEngine;

namespace Services.Ads
{
    public class InterstitialAdSample
    {
        private LevelPlayInterstitialAd ad;

        public InterstitialAdSample()
        {
            ad = new LevelPlayInterstitialAd("0ot85tyr3a7uy2qe");

            ad.OnAdClosed += _ => onClosedCallback?.Invoke();
            ad.OnAdLoaded += _ => Debug.Log("Interstitial Loaded");
            ad.OnAdLoadFailed += _ => Debug.Log("Interstitial Load Failed");

            Load();
        }

        public void Load() => ad.LoadAd();

        public bool IsReady() => ad.IsAdReady();

        private System.Action onClosedCallback;

        public void Show(System.Action onClosed)
        {
            if (!IsReady()) return;

            onClosedCallback = onClosed;
            ad.ShowAd();
        }
    }
}