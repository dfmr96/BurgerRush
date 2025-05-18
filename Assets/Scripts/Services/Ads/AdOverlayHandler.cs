using System;
using UnityEngine;

public static class AdOverlayHandler
{
    /// <summary>
    /// Muestra un interstitial y oculta/restaura el banner si es necesario.
    /// </summary>
    /// <param name="onAdFinished">Acción a ejecutar después del cierre del ad.</param>
    public static void ShowInterstitialWithBannerRestore(Action onAdFinished)
    {
        bool wasBannerVisible = AdsManager.Instance.IsBannerVisible();

        AdsManager.Instance.HideBanner();

        AdsManager.Instance.TryShowInterstitial(() =>
        {
            if (wasBannerVisible)
                AdsManager.Instance.ShowBanner();

            onAdFinished?.Invoke();
        });
    }

    /// <summary>
    /// Muestra un rewarded ad y oculta/restaura el banner si es necesario.
    /// </summary>
    /// <param name="onRewardGranted">Acción a ejecutar cuando el reward sea otorgado.</param>
    public static void ShowRewardedWithBannerRestore(Action onRewardGranted)
    {
        bool wasBannerVisible = AdsManager.Instance.IsBannerVisible();

        AdsManager.Instance.HideBanner();

        AdsManager.Instance.TryShowRewarded(() =>
        {
            if (wasBannerVisible)
                AdsManager.Instance.ShowBanner();

            onRewardGranted?.Invoke();
        });
    }
}