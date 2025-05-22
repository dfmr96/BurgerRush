using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Databases;
using Services;
using Services.Cloud;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class GameBootstrapper : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────────
    // 📣 Public API
    // ─────────────────────────────────────────────────────────────

    public static bool IsReady { get; private set; }
    public static event Action OnBootstrapComplete;

    // ─────────────────────────────────────────────────────────────
    // 🔧 Config & Refs
    // ─────────────────────────────────────────────────────────────

    [SerializeField] private string nextSceneName = "MainMenu";
    
    [Header("References")]
    [SerializeField] private PlayerStatsDatabase statsDatabase;
    [SerializeField] private LoadingUI loadingUI;

    [Header("Options")]
    [SerializeField] private bool useDevMode = false;

    // [Header("Optional UI")]
    // [SerializeField] private LoadingUI loadingUI;

    // ─────────────────────────────────────────────────────────────
    // 🚀 Bootstrap Lifecycle
    // ─────────────────────────────────────────────────────────────

    private async void Awake()
    {
        DontDestroyOnLoad(gameObject);
        await InitializeServicesAsync();
    }

    private async Task InitializeServicesAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        Debug.Log("🚀 Starting bootstrap...");

        // UGS
        Debug.Log("🛠️ Initializing Unity Gaming Services...");
        loadingUI.SetProgress(0.1f);
        await UgsInitializer.InitializeUGSAsync();
        Debug.Log("✅ UGS Initialized");
        loadingUI.SetProgress(0.3f);

        // Cloud Sync
        if (!useDevMode)
        {
            Debug.Log("🔄 Syncing cloud data...");
            await CloudSyncService.ValidateCloudSync(statsDatabase);
            Debug.Log("✅ Cloud Sync complete");
        }
        else
        {
            Debug.Log("🧪 Dev mode: Skipping cloud sync");
        }
        loadingUI.SetProgress(0.6f);
        // Ads
        EnsureAdsManager();
        loadingUI.SetProgress(0.9f);

        if (!useDevMode)
        {
            Debug.Log("📢 Waiting for Ads to initialize...");
            await AdsManager.InstanceWaitUntilReady();
            Debug.Log("✅ Ads Initialized");
        }
        else
        {
            Debug.Log("🧪 Dev mode: Skipping Ads init");
        }

        stopwatch.Stop();
        IsReady = true;
        OnBootstrapComplete?.Invoke();
        
        loadingUI.SetMessage("Loaded.");
        loadingUI.SetProgress(1.0f);

        Debug.Log($"🏁 All systems go. Boot time: {stopwatch.ElapsedMilliseconds} ms");
        await Task.Delay(2000);
        SceneManager.LoadScene(nextSceneName);
    }

    // ─────────────────────────────────────────────────────────────
    // 🔧 Helpers
    // ─────────────────────────────────────────────────────────────

    private void EnsureAdsManager()
    {
        if (AdsManager.Instance == null)
        {
            var obj = new GameObject("AdsManager");
            obj.AddComponent<AdsManager>();
            DontDestroyOnLoad(obj);
        }
    }
}
