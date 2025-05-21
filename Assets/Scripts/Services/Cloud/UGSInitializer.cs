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
        private TaskCompletionSource<bool> _authCompletion = new();
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

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    Debug.Log("⌛ Waiting for GooglePlayAuthenticator.OnSignedIn...");
                    GooglePlayAuthenticator.OnSignedIn += HandleSignedIn;
                    await _authCompletion.Task;
                }

                StartAnalytics();

                IsCloudAvailable = true;
                OnUGSReady?.Invoke();
                Debug.Log("✅ UGS Initialization complete.");
            }
            catch (Exception e)
            {
                IsCloudAvailable = false;
                Debug.LogError($"❌ UGS Initialization failed: {e.Message}");
            }
        }
        
        private void HandleSignedIn()
        {
            GooglePlayAuthenticator.OnSignedIn -= HandleSignedIn;
            _authCompletion.TrySetResult(true);
        }

        private async Task InitializeUnityServicesAsync()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                await UnityServices.InitializeAsync();
                Debug.Log("✅ Unity Services initialized.");
            }
        }

        private void StartAnalytics()
        {
            AnalyticsService.Instance.StartDataCollection();
            Debug.Log("📈 Unity Analytics started.");
        }
    }
}
