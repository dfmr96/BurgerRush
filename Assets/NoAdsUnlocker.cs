using Services.Ads;
using Services.Cloud;
using UnityEngine;
using UnityEngine.UI;

public class NoAdsUnlocker : MonoBehaviour
{
    [SerializeField] private bool hideBannerAfterPurchase = true;
    [SerializeField] private Button noadsButton;

    private void OnEnable()
    {
        if (NoAdsService.IsInitialized)
        {
            CheckNoAds(); // consulta directa
        }
        else
        {
            UgsInitializer.OnUGSReady += CheckNoAds; // fallback si por alguna razón no está listo
        }

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
        if (NoAdsService.HasNoAds)
        {
            HideButton();
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