using Unity.Services.LevelPlay;
using UnityEngine;

public class BannerAdController : MonoBehaviour 
{
  #if UNITY_ANDROID
    private const string appKey = "85460dcd";
    private const string bannerAdUnitId = "2ysgx5mehskfdlh3";
#elif UNITY_IOS
    private const string appKey = "8545d445";
    private const string bannerAdUnitId = "iep3rxsyp9na3rw8"; // Cambialo por el real si usás iOS
#else
    private const string appKey = "85460dcd";
    private const string bannerAdUnitId = "2ysgx5mehskfdlh3";
#endif

    private LevelPlayBannerAd bannerAd;

    private void Start()
    {
        Debug.Log("👀 Start() de BannerAdController llamado");
        InitLevelPlaySDK();
    }

    private void InitLevelPlaySDK()
    {
        Debug.Log("🛠️ Inicializando LevelPlay SDK con AppKey: " + appKey);

        IronSource.Agent.setAdaptersDebug(true);
        IronSource.Agent.validateIntegration();

        LevelPlay.Init(appKey);
        LevelPlay.OnInitSuccess += OnInitSuccess;
        LevelPlay.OnInitFailed += OnInitFailed;
    }

    private void OnInitSuccess(LevelPlayConfiguration config)
    {
        Debug.Log("✅ LevelPlay inicializado correctamente.");
        SetupAndLoadBanner();
    }

    private void OnInitFailed(LevelPlayInitError error)
    {
        Debug.LogError("❌ Error al inicializar LevelPlay: " + error);
    }

    private void SetupAndLoadBanner()
    {
        bannerAd = new LevelPlayBannerAd(bannerAdUnitId);

        // Eventos
        bannerAd.OnAdLoaded += info => Debug.Log("✅ Banner cargado correctamente.");
        bannerAd.OnAdLoadFailed += error => Debug.LogWarning("⚠️ Error al cargar banner: " + error);
        bannerAd.OnAdDisplayed += info => Debug.Log("🎉 Banner mostrado.");
        bannerAd.OnAdClicked += info => Debug.Log("👆 Banner clickeado.");
        bannerAd.OnAdDisplayFailed += error => Debug.LogWarning("🚫 Falló el display del banner: " + error);

        // Cargar el banner
        bannerAd.LoadAd();
    }

    public void HideBanner()
    {
        bannerAd?.HideAd();
        Debug.Log("🫥 Banner ocultado.");
    }

    public void ShowBanner()
    {
        bannerAd?.ShowAd();
        Debug.Log("📢 Banner mostrado manualmente.");
    }

    private void OnDisable()
    {
        bannerAd?.DestroyAd();
    }
}

