using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.LevelPlay;
using Services.Cloud;
using Services.Ads;

public class AdsManager : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────────
    // 🔗 Singleton
    // ─────────────────────────────────────────────────────────────

    public static AdsManager Instance { get; private set; }

    // ─────────────────────────────────────────────────────────────
    // 📣 Public State
    // ─────────────────────────────────────────────────────────────

    public event Action OnInitialized;
    public bool IsInitialized { get; private set; }

    private int SessionPlays
    {
        get => PlayerPrefs.GetInt(PlayCountKey, 0);
        set
        {
            PlayerPrefs.SetInt(PlayCountKey, value);
            PlayerPrefs.Save();
        }
    }

    // ─────────────────────────────────────────────────────────────
    // 📦 Serialized Fields
    // ─────────────────────────────────────────────────────────────

    [Header("LevelPlay App Key")]
    [SerializeField] private string appKey = "220ee44fd";

    // ─────────────────────────────────────────────────────────────
    // 🔐 Private Fields
    // ─────────────────────────────────────────────────────────────

    private const string PlayCountKey = "TotalSessionPlays";

    private BannerAdSample banner;
    private InterstitialAdSample interstitial;
    private RewardedAdSample rewarded;

    private bool bannerVisible = false;

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
        UgsInitializer.OnUGSReady += InitializeAds;
    }

    private void OnEnable()
    {
        LevelPlay.OnInitSuccess += OnInitSuccess;
        LevelPlay.OnInitFailed += OnInitFailed;
    }

    private void OnDisable()
    {
        UgsInitializer.OnUGSReady -= InitializeAds;
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
    // 🚀 Initialization
    // ─────────────────────────────────────────────────────────────

    private async void InitializeAds()
    {
        await NoAdsService.InitializeAsync();

        if (NoAdsService.HasNoAds)
        {
            Debug.Log("🚫 Ads disabled by NoAds purchase.");
            return;
        }

        if (IsInitialized) return;

        Debug.Log("🎯 UGS and NoAds ready. Initializing ads...");

        LevelPlayBootstrapper.Initialize(
            appKey,
            OnInitSuccess,
            OnInitFailed
        );
    }

    private void OnInitSuccess(LevelPlayConfiguration config)
    {
        Debug.Log("✅ LevelPlay initialized");

        banner = new BannerAdSample();
        interstitial = new InterstitialAdSample();
        rewarded = new RewardedAdSample();

        IsInitialized = true;
        OnInitialized?.Invoke();
    }

    private void OnInitFailed(LevelPlayInitError error)
    {
        Debug.LogError("❌ LevelPlay init failed: " + error);
    }
    
    public static async Task InstanceWaitUntilReady()
    {
        while (Instance == null || !Instance.IsInitialized)
            await Task.Yield();
    }

    // ─────────────────────────────────────────────────────────────
    // 📢 Banner Methods
    // ─────────────────────────────────────────────────────────────

    public void ShowBanner()
    {
#if UNITY_WEBGL
        Debug.Log("🚫 Banner ads not supported in WebGL.");
        return;
#endif
        if (ShouldBlockAds("banner")) return;
        if (banner == null)
        {
            Debug.LogWarning("⚠️ Banner ad not initialized.");
            return;
        }
        if (bannerVisible)
        {
            Debug.Log("ℹ️ Banner already visible.");
            return;
        }

        banner.Show();

        if (banner.IsReady())
        {
            bannerVisible = true;
            Debug.Log("✅ Banner is now visible.");
        }
        else
        {
            Debug.Log("⏳ Banner not ready.");
        }
    }

    public void HideBanner()
    {
        if (banner == null || !bannerVisible)
        {
            Debug.Log("ℹ️ Banner already hidden or not initialized.");
            return;
        }

        banner.Hide();
        bannerVisible = false;
    }

    public bool IsBannerReady() => banner != null && banner.IsReady();
    public bool IsBannerVisible() => bannerVisible;

    // ─────────────────────────────────────────────────────────────
    // 🎮 Interstitial Ads
    // ─────────────────────────────────────────────────────────────

    public void TryShowInterstitial(Action onFinished)
    {
#if UNITY_WEBGL
        Debug.Log("🚫 Interstitial ads not supported in WebGL.");
        onFinished?.Invoke();
        return;
#endif
        if (ShouldBlockAds("interstitial")) { onFinished?.Invoke(); return; }

        if (SessionPlays <= 3 || interstitial == null || !interstitial.IsReady())
        {
            Debug.Log("ℹ️ Interstitial skipped.");
            onFinished?.Invoke();
            return;
        }

        interstitial.Show(() =>
        {
            interstitial.Load(); // Preload next
            onFinished?.Invoke();
        });
    }

    // ─────────────────────────────────────────────────────────────
    // 🎁 Rewarded Ads
    // ─────────────────────────────────────────────────────────────

    public bool IsRewardedReady() => rewarded != null && rewarded.IsReady();

    public void TryShowRewarded(Action onRewardGranted)
    {
#if UNITY_WEBGL
        Debug.Log("🚫 Rewarded ads not supported in WebGL.");
        return;
#endif
        if (ShouldBlockAds("rewarded")) return;

        if (!IsRewardedReady())
        {
            Debug.Log("⏳ Rewarded ad not ready.");
            return;
        }

        rewarded.Show(onRewardGranted);
    }

    // ─────────────────────────────────────────────────────────────
    // 📈 Session Tracking
    // ─────────────────────────────────────────────────────────────

    public void IncrementPlayCount() => SessionPlays++;

    // ─────────────────────────────────────────────────────────────
    // 🔒 Helpers
    // ─────────────────────────────────────────────────────────────

    private bool ShouldBlockAds(string type)
    {
        if (NoAdsService.HasNoAds)
        {
            Debug.Log($"🚫 {type} ad blocked by NoAds purchase.");
            return true;
        }
        return false;
    }
}
