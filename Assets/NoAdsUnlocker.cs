using System.Threading.Tasks;
using Services.Ads;
using Services.Cloud;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Product = UnityEngine.Purchasing.Product;

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

    public void UnlockNoAds(Product product)
    {
        if (product.definition.id != "noads") return;

        _ = UnlockNoAdsAsync(); // Disparar sin await, no bloquea
    }

    private async Task UnlockNoAdsAsync()
    {
        await NoAdsService.UnlockNoAdsAsync();

        if (hideBannerAfterPurchase && AdsManager.Instance.IsBannerVisible())
        {
            AdsManager.Instance.HideBanner();
        }
    }

    private void CheckNoAds()
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