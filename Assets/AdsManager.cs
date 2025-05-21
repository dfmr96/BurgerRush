using System;
using Services.Ads;
using Services.Cloud;
using UnityEngine;
using Unity.Services.LevelPlay;
using Mediation = com.unity3d.mediation;

public class AdsManager : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────────
    // 🔗 Singleton
    // ─────────────────────────────────────────────────────────────

    public static AdsManager Instance { get; private set; }

    // ─────────────────────────────────────────────────────────────
    // 📦 Serialized Fields
    // ─────────────────────────────────────────────────────────────

    [Header("App Keys")] [SerializeField] private string appKey = "220ee44fd";

    // ─────────────────────────────────────────────────────────────
    // 📣 Public State
    // ─────────────────────────────────────────────────────────────

    public event Action OnInitialized;
    public bool IsInitialized { get; private set; }


    // ─────────────────────────────────────────────────────────────
    // 📦 Private Fields
    // ─────────────────────────────────────────────────────────────

    private BannerAdSample bannerAd;
    private InterstitialAdSample interstitialAd;
    private RewardedAdSample rewardedAd;

    private const string PlayCountKey = "TotalSessionPlays";

    public int SessionPlays
    {
        get => PlayerPrefs.GetInt(PlayCountKey, 0);
        private set
        {
            PlayerPrefs.SetInt(PlayCountKey, value);
            PlayerPrefs.Save();
        }
    }

    private bool isBannerVisible = false;

    // ─────────────────────────────────────────────────────────────
    // 🔧 Unity Lifecycle
    // ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        UGSInitializer.OnUGSReady += InitializeAds;
    }


    private void OnEnable()
    {
        LevelPlay.OnInitSuccess += OnInitSuccess;
        LevelPlay.OnInitFailed += OnInitFailed;
    }

    private void OnDisable()
    {
        UGSInitializer.OnUGSReady -= InitializeAds;
        LevelPlay.OnInitSuccess -= OnInitSuccess;
        LevelPlay.OnInitFailed -= OnInitFailed;
    }

    private void OnApplicationPause(bool isPaused)
    {
#if !UNITY_WEBGL
        IronSource.Agent.onApplicationPause(isPaused);
#endif
    }

    // ─────────────────────────────────────────────────────────────
    // 🧠 Initialization Callbacks
    // ─────────────────────────────────────────────────────────────


    private void OnInitSuccess(LevelPlayConfiguration config)
    {
        Debug.Log("✅ LevelPlay initialized");

        bannerAd = new BannerAdSample();
        interstitialAd = new InterstitialAdSample();
        rewardedAd = new RewardedAdSample();

        IsInitialized = true;
        OnInitialized?.Invoke();
    }

    private void OnInitFailed(LevelPlayInitError error)
    {
        Debug.LogError("❌ LevelPlay init failed: " + error);
    }


    // ─────────────────────────────────────────────────────────────
    // 📢 Public Methods
    // ─────────────────────────────────────────────────────────────

    public void SetNoAds()
    {
        AdsSettings.SetNoAds(true);
        HideBanner(); // por si ya estaba activo
    }
    public bool IsRewardedReady() => rewardedAd?.IsReady() == true;

    public void ShowBanner()
    {
#if UNITY_WEBGL
    Debug.Log("🚫 Banner ads not supported in WebGL.");
    return;
#endif
        if (AdsSettings.HasNoAds())
        {
            Debug.Log("🚫 Ads disabled by NoAds purchase.");
            return;
        }
        
        if (bannerAd == null)
        {
            Debug.Log("⚠️ bannerAd is null.");
            return;
        }

        if (isBannerVisible)
        {
            Debug.Log("ℹ️ Banner already visible.");
            return;
        }

        Debug.Log("📢 Attempting to show banner...");
        bannerAd.Show();

        // Solo marcamos como visible si sabemos que se pudo mostrar
        if (bannerAd.IsReady()) 
        {
            isBannerVisible = true;
            Debug.Log("✅ Banner is now visible.");
        }
        else
        {
            Debug.Log("⏳ Banner not ready yet.");
        }

    }

    public void HideBanner()
    {
        if (bannerAd == null || !isBannerVisible)
        {
            Debug.Log("ℹ️ Banner already hidden or not initialized.");
            return;
        }

        bannerAd.Hide();
        isBannerVisible = false;
    }
    
    public bool IsBannerReady()
    {
        return bannerAd != null && bannerAd.IsReady();
    }

    public bool IsBannerVisible() => isBannerVisible;

    public void IncrementPlayCount() => SessionPlays++;

    public void TryShowInterstitial(Action onFinished)
    {
#if UNITY_WEBGL
        Debug.Log("🚫 Ads not supported in WebGL.");
        onFinished?.Invoke();
        return;
#endif
        if (AdsSettings.HasNoAds())
        {
            Debug.Log("🚫 Ads disabled by NoAds purchase.");
            onFinished?.Invoke();
            return;
        }

        if (SessionPlays <= 3 || !interstitialAd?.IsReady() == true)
        {
            onFinished?.Invoke();
            return;
        }

        interstitialAd.Show(() =>
        {
            interstitialAd.Load();
            onFinished?.Invoke();
        });
    }

    public void TryShowRewarded(Action onRewardGranted)
    {
#if UNITY_WEBGL
        Debug.Log("🚫 Rewarded ads not supported in WebGL.");
        return;
#endif
        if (AdsSettings.HasNoAds())
        {
            Debug.Log("🚫 Rewarded ads disabled by NoAds purchase.");
            return;
        }

        if (!rewardedAd?.IsReady() == true)
            return;

        rewardedAd.Show(onRewardGranted);

    }

    // ─────────────────────────────────────────────────────────────
    // 📢 Private Methods
    // ─────────────────────────────────────────────────────────────

    private void InitializeAds()
    {
#if UNITY_WEBGL
        Debug.Log("🚫 Skipping LevelPlay initialization in WebGL.");
        return;
#else

        if (IsInitialized) return;
        Debug.Log("🎯 UGS ready. Initializing ads...");
        LevelPlay.Init(appKey);
#endif
    }
}
