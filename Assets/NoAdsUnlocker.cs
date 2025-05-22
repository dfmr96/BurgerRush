using System;
using Services.Ads;
using Services.Cloud;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class NoAdsUnlocker : MonoBehaviour
{
    [SerializeField] private bool hideBannerAfterPurchase = true;
    [SerializeField] private Button noadsButton;

    private void OnEnable()
    {
        UgsInitializer.OnUGSReady += CheckNoAds;
        NoAdsService.OnNoAdsUnlocked += HideButton;
    }

    private void OnDisable()
    {
        UgsInitializer.OnUGSReady -= CheckNoAds;
        NoAdsService.OnNoAdsUnlocked -= HideButton;
    }

    public async void UnlockNoAds()
    {
        await NoAdsService.UnlockNoAdsAsync();

        if (hideBannerAfterPurchase && AdsManager.Instance.IsBannerVisible())
        {
            AdsManager.Instance.HideBanner();
        }

        // ⚠️ Ya no hace falta ocultar el botón aquí
        // Lo hará el evento automáticamente
    }

    private async void CheckNoAds()
    {
        await NoAdsService.InitializeAsync();

        if (NoAdsService.HasNoAds)
        {
            HideButton(); // ← reactivo
        }
    }

    private void HideButton()
    {
        if (noadsButton != null && noadsButton.gameObject.activeSelf)
        {
            noadsButton.gameObject.SetActive(false);
            Debug.Log("🛑 No Ads button hidden via NoAdsService event.");
        }
    }
}