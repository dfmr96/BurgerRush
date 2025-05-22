using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

namespace Services.Cloud
{
    public static class UgsInitializer
    {
        private static bool IsCloudAvailable { get; set; }

        public static event Action OnUGSReady
        {
            add
            {
                if (IsCloudAvailable)
                    value?.Invoke();
                else
                    _onUGSReady += value;
            }
            remove => _onUGSReady -= value;
        }
        private static event Action _onUGSReady;

        private static TaskCompletionSource<bool> _initializationTcs = new();

        public static async Task InitializeUGSAsync()
        {
            try
            {
                await InitializeUnityServicesAsync();

#if UNITY_EDITOR
                Debug.Log("🧪 Editor detected. Signing in anonymously...");
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                GooglePlayAuthenticator.ForceRaiseSignedIn();
#else
        Debug.Log("⌛ Waiting for GooglePlayAuthenticator sign-in...");
        var tcs = new TaskCompletionSource<bool>();

        void OnSignedInHandler()
        {
            GooglePlayAuthenticator.OnSignedIn -= OnSignedInHandler;
            tcs.TrySetResult(true);
        }

        GooglePlayAuthenticator.OnSignedIn += OnSignedInHandler;
        await tcs.Task;
#endif

                StartAnalytics();

                IsCloudAvailable = true;
                _initializationTcs.TrySetResult(true);
                _onUGSReady?.Invoke();

                Debug.Log("✅ UGS Initialization complete.");
            }
            catch (Exception e)
            {
                IsCloudAvailable = false;
                _initializationTcs.TrySetException(e);
                Debug.LogError($"❌ UGS Initialization failed: {e.Message}");
            }
        }


        private static async Task InitializeUnityServicesAsync()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                await UnityServices.InitializeAsync();
                Debug.Log("✅ Unity Services initialized.");
            }
        }

        private static void StartAnalytics()
        {
            AnalyticsService.Instance.StartDataCollection();
            Debug.Log("📈 Unity Analytics started.");
        }

        /// <summary>
        /// Called by any system that needs to wait until UGS + authentication is ready.
        /// </summary>
        public static async Task EnsureInitializedAsync()
        {
            await _initializationTcs.Task;
        }
    }
}
