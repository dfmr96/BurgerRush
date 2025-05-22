using Services.Ads;
using UnityEngine;

public class GameplayAdsController : MonoBehaviour
{
    private bool bannerShown = false;
    [SerializeField] private GameObject freeContinueIcon;

    private void Start()
    {
        if (freeContinueIcon != null)
        {
            freeContinueIcon.SetActive(NoAdsService.HasNoAds);
        }
        
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
        if (!bannerShown)
        {
            AdsManager.Instance.ShowBanner();
            bannerShown = true;
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