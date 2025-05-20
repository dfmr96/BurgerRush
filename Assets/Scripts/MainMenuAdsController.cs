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
        // Espera hasta que esté cargado el banner
        var bannerReady = false;

        while (!bannerReady)
        {
            if (AdsManager.Instance.IsBannerVisible())
            {
                Debug.Log("🟢 Banner ya visible.");
                yield break;
            }

            bannerReady = AdsManager.Instance.IsInitialized && AdsManager.Instance.IsBannerReady();
            if (!bannerReady)
                yield return new WaitForSeconds(0.5f); // espera medio segundo
        }

        AdsManager.Instance.ShowBanner();
    }
}