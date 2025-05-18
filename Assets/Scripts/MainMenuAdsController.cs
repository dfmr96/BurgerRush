using UnityEngine;

public class MainMenuAdsController : MonoBehaviour
{
    private void Start()
    {
        if (AdsManager.Instance.IsInitialized)
        {
            AdsManager.Instance.ShowBanner();
        }
        else
        {
            AdsManager.Instance.OnInitialized += HandleAdsReady;
        }
    }

    private void HandleAdsReady()
    {
        AdsManager.Instance.OnInitialized -= HandleAdsReady;
        AdsManager.Instance.ShowBanner();
    }
}