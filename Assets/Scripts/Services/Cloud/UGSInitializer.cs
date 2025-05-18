using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Analytics;
using UnityEngine;

namespace Services.Cloud
{
    public class UGSInitializer : MonoBehaviour
    {
        public static event Action OnUGSReady;

        public static bool IsCloudAvailable { get; private set; } = false;

        private async void Awake()
        {
            await InitializeServicesAsync();
        }

        private async Task InitializeServicesAsync()
        {
            try
            {
                await InitializeUnityServicesAsync();
                await TryAuthenticateAsync();
                StartAnalytics();

                IsCloudAvailable = true;
                OnUGSReady?.Invoke();
            }
            catch (Exception e)
            {
                IsCloudAvailable = false;
                Debug.LogError($"❌ UGS Initialization failed: {e.Message}");
            }
        }

        private async Task InitializeUnityServicesAsync()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                await UnityServices.InitializeAsync();
                Debug.Log("✅ Unity Services initialized.");
            }
        }

        private async Task TryAuthenticateAsync()
        {
            try
            {
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    Debug.Log($"🔐 Signed in. PlayerID: {AuthenticationService.Instance.PlayerId}");
                }
            }
            catch (Exception authEx)
            {
                Debug.LogWarning($"⚠️ Authentication skipped (likely WebGL): {authEx.Message}");
            }
        }

        private void StartAnalytics()
        {
            AnalyticsService.Instance.StartDataCollection();
            Debug.Log("📈 Unity Analytics started.");
        }
    }
}
