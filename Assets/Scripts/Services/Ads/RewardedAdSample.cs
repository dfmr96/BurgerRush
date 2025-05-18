using Unity.Services.LevelPlay;
using UnityEngine;
using UnityEngine.UI;

namespace Services.Ads
{
    public class RewardedAdSample
    {
        private LevelPlayRewardedAd rewardedAd;
        private Button loadRewardedAd;
        
        public RewardedAdSample()
        {
            //Create RewardedAd instance
            CreateRewardedAd();
        }

        private void CreateRewardedAd()
        {
            rewardedAd = new LevelPlayRewardedAd("hprkhi532fb72m6k");
            
            //Subscribe RewardedAd events
            rewardedAd.OnAdClosed += RewardedOnAdClosedEvent;
            rewardedAd.OnAdInfoChanged += RewardedOnAdInfoChangedEvent;
            rewardedAd.OnAdLoaded += RewardedOnAdLoadedEvent;
            rewardedAd.OnAdDisplayed += RewardedOnAdDisplayedEvent;
            rewardedAd.OnAdClicked += RewardedOnAdClickedEvent;
            rewardedAd.OnAdRewarded += RewardedOnAdRewardedEvent;
            rewardedAd.OnAdDisplayFailed += RewardedOnAdDisplayFailedEvent;
            rewardedAd.OnAdLoadFailed += RewardedOnAdLoadFailedEvent;
        }

        public void LoadRewardedAd(Button showButton)
        {
            rewardedAd.LoadAd();
            loadRewardedAd = showButton;
        }
        
        public void ShowRewardedAd()
        {
            //Show RewardedAd, check if the ad is ready before showing
            if (rewardedAd.IsAdReady())
            {
                rewardedAd.ShowAd();
            }
        }
        
        private void DestroyAd()
        {
            //Destroy RewardedAd 
            rewardedAd.DestroyAd();
        }
        private void RewardedOnAdLoadFailedEvent(LevelPlayAdError obj)
        {
        }

        private void RewardedOnAdDisplayFailedEvent(LevelPlayAdDisplayInfoError obj)
        {
        }

        private void RewardedOnAdRewardedEvent(LevelPlayAdInfo arg1, LevelPlayReward arg2)
        {
            Debug.Log("Rewarded ad rewarded");
        }

        private void RewardedOnAdClickedEvent(LevelPlayAdInfo obj)
        {
        }

        private void RewardedOnAdDisplayedEvent(LevelPlayAdInfo obj)
        {
        }

        private void RewardedOnAdInfoChangedEvent(LevelPlayAdInfo obj)
        {
        }

        private void RewardedOnAdLoadedEvent(LevelPlayAdInfo obj)
        {
            loadRewardedAd.interactable = true;
        }

        private void RewardedOnAdClosedEvent(LevelPlayAdInfo obj)
        {
        }
    }
}