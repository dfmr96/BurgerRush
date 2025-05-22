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
    }

    private void OnDisable()
    {
        UgsInitializer.OnUGSReady -= CheckNoAds;
    }

    public async void UnlockNoAds()
    {
        // 1. Guardar localmente
        AdsSettings.SetNoAds(true);

        // 2. Guardar en la nube
        await AdsSettings.SaveNoAdsToCloud();

        // 3. Ocultar banner si está visible
        if (hideBannerAfterPurchase && AdsManager.Instance.IsBannerVisible())
        {
            AdsManager.Instance.HideBanner();
        }

        // 4. Ocultar botón
        noadsButton.gameObject.SetActive(false);

        Debug.Log("✅ No Ads unlocked and saved to cloud.");
    }

    private async void CheckNoAds()
    {
        await NoAdsService.InitializeAsync(); // garantiza que ya se leyó de la nube

        if (NoAdsService.HasNoAds)
        {
            noadsButton.gameObject.SetActive(false);
        }
    }
}