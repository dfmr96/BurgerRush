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
                await InitializeUnityServicesAsync(); // 🔑 UnityServices ya está inicializado

#if UNITY_EDITOR
                Debug.Log("🧪 Editor detected. Signing in anonymously...");
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                GooglePlayAuthenticator.ForceRaiseSignedIn();
#else
        // 🔐 Esperar a GooglePlayAuthenticator (que ahora ya puede usar AuthenticationService)
        Debug.Log("⌛ Waiting for GooglePlayAuthenticator sign-in...");

        // Si ya hay sesión, no esperes
        if (AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("🙌 Already signed in, skipping GPGS wait.");
        }
        else
        {
            // Forzar login automático
            GooglePlayAuthenticator.Instance?.TryAutoLogin();

            var tcs = new TaskCompletionSource<bool>();

            void OnSignedInHandler()
            {
                GooglePlayAuthenticator.OnSignedIn -= OnSignedInHandler;
                tcs.TrySetResult(true);
            }

            GooglePlayAuthenticator.OnSignedIn += OnSignedInHandler;
            await tcs.Task;
        }
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
