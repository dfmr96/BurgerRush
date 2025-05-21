using Services.Ads;
using UnityEngine;
using UnityEngine.UI;

public class NoAdsUnlocker : MonoBehaviour
{
    [SerializeField] private bool hideBannerAfterPurchase = true;
    [SerializeField] private Button noadsButton;

    private void Start()
    {
        // Si ya compró NoAds, ocultamos el botón
        if (AdsSettings.HasNoAds())
        {
            noadsButton.gameObject.SetActive(false);
        }
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
}