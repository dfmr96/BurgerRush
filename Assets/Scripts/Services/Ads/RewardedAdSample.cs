using Unity.Services.LevelPlay;
using UnityEngine;
using System;

namespace Services.Ads
{
    public class RewardedAdSample
    {
        private readonly LevelPlayRewardedAd ad;
        private Action onRewardedCallback;

        public RewardedAdSample()
        {
            ad = new LevelPlayRewardedAd("hprkhi532fb72m6k");

            ad.OnAdRewarded += HandleAdRewarded;
            ad.OnAdClosed += HandleAdClosed;
            ad.OnAdLoaded += _ => Debug.Log("✅ Rewarded ad loaded.");
            ad.OnAdLoadFailed += _ => Debug.LogWarning("❌ Rewarded ad load failed.");

            Load();
        }

        private void HandleAdRewarded(LevelPlayAdInfo info, com.unity3d.mediation.LevelPlayReward reward)
        {
            Debug.Log("🎁 Reward granted.");
            onRewardedCallback?.Invoke();
            onRewardedCallback = null;
        }

        private void HandleAdClosed(LevelPlayAdInfo info)
        {
            Debug.Log("🔄 Rewarded ad closed. Reloading...");
            Load();
        }

        public void Load() => ad.LoadAd();

        public bool IsReady() => ad.IsAdReady();

        public void Show(Action onRewarded)
        {
            if (!IsReady())
            {
                Debug.LogWarning("⚠️ Tried to show rewarded ad before ready.");
                return;
            }

            onRewardedCallback = onRewarded;
            ad.ShowAd();
        }
    }
}