using Unity.Services.LevelPlay;
using UnityEngine;

namespace Services.Ads
{
    public class RewardedAdSample
    {
        private LevelPlayRewardedAd ad;
        private System.Action onRewardedCallback;

        public RewardedAdSample()
        {
            ad = new LevelPlayRewardedAd("hprkhi532fb72m6k");

            ad.OnAdRewarded += (_, _) =>
            {
                Debug.Log("Rewarded granted!");
                onRewardedCallback?.Invoke();
            };

            ad.OnAdLoaded += _ => Debug.Log("Rewarded Loaded");
            ad.OnAdLoadFailed += _ => Debug.Log("Rewarded Load Failed");

            Load();
        }

        public void Load() => ad.LoadAd();

        public bool IsReady() => ad.IsAdReady();

        public void Show(System.Action onRewarded)
        {
            if (!IsReady()) return;

            onRewardedCallback = onRewarded;
            ad.ShowAd();
        }
    }
}