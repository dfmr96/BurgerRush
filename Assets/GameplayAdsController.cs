using UnityEngine;

public class GameplayAdsController : MonoBehaviour
{
    private bool bannerShown = false;

    private void Start()
    {
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