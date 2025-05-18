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

    private int sessionPlays = 0;

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

    public void ShowBanner() => bannerAd?.Show();
    public void HideBanner() => bannerAd?.Hide();

    public void IncrementPlayCount() => sessionPlays++;

    public void TryShowInterstitial(Action onFinished)
    {
        if (sessionPlays <= 3 || !interstitialAd?.IsReady() == true)
        {
            onFinished?.Invoke();
            return;
        }

        interstitialAd.Show(() =>
        {
            interstitialAd.Load(); // Prepare next
            onFinished?.Invoke();
        });
    }

    public void TryShowRewarded(Action onRewardGranted)
    {
        if (!rewardedAd?.IsReady() == true)
            return;

        rewardedAd.Show(onRewardGranted);
        rewardedAd.Load(); // Prepare next
    }
}
