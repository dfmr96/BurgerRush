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

    [Header("References")] [SerializeField]
    private PlayerStatsDatabase statsDatabase;

    [SerializeField] private LoadingUI loadingUI;

    [Header("Options")] [SerializeField] private bool useDevMode = false;

    // [Header("Optional UI")]
    // [SerializeField] private LoadingUI loadingUI;

    // ─────────────────────────────────────────────────────────────
    // 🚀 Bootstrap Lifecycle
    // ─────────────────────────────────────────────────────────────

    private async void Awake()
    {
        DontDestroyOnLoad(gameObject);
        await InitializeServicesAsync();
        await LoadNextSceneAsync();
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

        // Inicializar stats locales
        PlayerStatsService.InitializeAllStats(statsDatabase);
        Debug.Log("📊 Player stats initialized");

        // Cloud Sync
        Debug.Log("🔄 Syncing cloud data...");
        await CloudSyncService.ValidateCloudSync(statsDatabase);
        Debug.Log("✅ Cloud Sync complete");

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
    }

    private async Task LoadNextSceneAsync()
    {
        var loadOperation = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);
        loadOperation.allowSceneActivation = false;

        // Esperar hasta que la escena esté lista para activar
        while (loadOperation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingUI.SetProgress(progress);
            await Task.Yield();
        }

        // Acceder a la escena antes de activarla
        Scene nextScene = SceneManager.GetSceneByName(nextSceneName);
        GameObject[] rootObjects = nextScene.GetRootGameObjects();

        // Desactivar todos los root GameObjects para ocultar visualmente la escena
        foreach (var go in rootObjects)
            go.SetActive(false);

        // Activar la escena (necesario para SetActiveScene)
        loadOperation.allowSceneActivation = true;

        // Esperar finalización real
        while (!loadOperation.isDone)
            await Task.Yield();

        // Establecer como escena activa
        SceneManager.SetActiveScene(nextScene);

        // Aquí ya terminó el bootstrap, ahora sí mostramos la UI de MainMenu
        foreach (var go in rootObjects)
            go.SetActive(true);

        // Finalmente, descargar la escena del bootstrapper
        Scene bootstrapScene = SceneManager.GetSceneByName("Bootstrap");
        if (bootstrapScene.IsValid())
            SceneManager.UnloadSceneAsync(bootstrapScene);
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