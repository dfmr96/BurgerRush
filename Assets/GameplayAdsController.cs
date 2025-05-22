using Services.Ads;
using UnityEngine;

public class GameplayAdsController : MonoBehaviour
{
    private bool bannerShown = false;
    [SerializeField] private GameObject freeContinueIcon;

    private void OnEnable()
    {
        NoAdsService.OnNoAdsUnlocked += HandleNoAdsUnlocked;
    }

    private void OnDisable()
    {
        NoAdsService.OnNoAdsUnlocked -= HandleNoAdsUnlocked;
    }

    private void Start()
    {
        UpdateFreeContinueIcon();

        if (AdsManager.Instance.IsInitialized)
        {
            ShowBannerIfNeeded();
        }
        else
        {
            AdsManager.Instance.OnInitialized += HandleAdsReady;
        }
    }

    private void HandleAdsReady()
    {
        AdsManager.Instance.OnInitialized -= HandleAdsReady;
        ShowBannerIfNeeded();
    }

    private void ShowBannerIfNeeded()
    {
        if (NoAdsService.HasNoAds || bannerShown)
            return;

        AdsManager.Instance.ShowBanner();
        bannerShown = true;
    }

    private void HandleNoAdsUnlocked()
    {
        if (bannerShown)
        {
            AdsManager.Instance.HideBanner();
            bannerShown = false;
        }

        UpdateFreeContinueIcon();
    }

    private void UpdateFreeContinueIcon()
    {
        if (freeContinueIcon != null)
        {
            freeContinueIcon.SetActive(NoAdsService.HasNoAds);
        }
    }

    private void OnDestroy()
    {
        if (bannerShown && AdsManager.Instance != null)
        {
            AdsManager.Instance.HideBanner();
            bannerShown = false;
        }
    }
}