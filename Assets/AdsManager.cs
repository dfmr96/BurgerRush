using System;
using Services.Ads;
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

    [Header("App Keys")]
    [SerializeField] private string appKey = "220ee44fd";

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
    }

    private void Start()
    {
        LevelPlay.Init(appKey);
    }

    private void OnEnable()
    {
        LevelPlay.OnInitSuccess += OnInitSuccess;
        LevelPlay.OnInitFailed += OnInitFailed;
    }

    private void OnDisable()
    {
        LevelPlay.OnInitSuccess -= OnInitSuccess;
        LevelPlay.OnInitFailed -= OnInitFailed;
    }

    private void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
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

    public bool IsRewardedReady() => rewardedAd?.IsReady() == true;
    public void ShowBanner()
    {
        if (bannerAd == null || isBannerVisible)
        {
            Debug.Log("ℹ️ Banner already visible or not initialized.");
            return;
        }

        bannerAd.Show();
        isBannerVisible = true;
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

    public bool IsBannerVisible() => isBannerVisible;

    public void IncrementPlayCount() => SessionPlays++;

    public void TryShowInterstitial(Action onFinished)
    {
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
        if (!rewardedAd?.IsReady() == true)
            return;

        rewardedAd.Show(onRewardGranted);
    }
    
    
}
