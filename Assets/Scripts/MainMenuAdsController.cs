using System.Collections;
using UnityEngine;

public class MainMenuAdsController : MonoBehaviour
{
    private void Start()
    {
        if (AdsManager.Instance.IsInitialized)
        {
            StartCoroutine(WaitForBannerAndShow());
        }
        else
        {
            AdsManager.Instance.OnInitialized += HandleAdsReady;
        }
    }

    private void HandleAdsReady()
    {
        AdsManager.Instance.OnInitialized -= HandleAdsReady;
        StartCoroutine(WaitForBannerAndShow());
    }

    private IEnumerator WaitForBannerAndShow()
    {
        while (!AdsManager.Instance.IsInitialized)
            yield return new WaitForSeconds(0.2f);

        if (!AdsManager.Instance.CanShowAds)
        {
            Debug.Log("🛑 Ads are disabled — no banner shown.");
            yield break;
        }

        if (AdsManager.Instance.IsBannerVisible())
        {
            Debug.Log("🟢 Banner ya visible.");
            yield break;
        }

        while (!AdsManager.Instance.IsBannerReady())
            yield return new WaitForSeconds(0.5f);

        AdsManager.Instance.ShowBanner();
    }
}