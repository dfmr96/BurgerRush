using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Services.Cloud
{
    public class UGSInitializer : MonoBehaviour
    {
        public static event Action OnUGSReady;

        public static bool IsCloudAvailable { get; private set; } = false;

        private async void Awake()
        {
#if !UNITY_WEBGL
            await InitializeServices();
#endif
}
        private async Task InitializeServices()
        {
            try
            {
                if (UnityServices.State != ServicesInitializationState.Initialized)
                {
                    await UnityServices.InitializeAsync();
                }

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                IsCloudAvailable = true;
                Debug.Log($"✅ Unity Services initialized. PlayerID: {AuthenticationService.Instance.PlayerId}");
                OnUGSReady?.Invoke();
            }
            catch (Exception e)
            {
                IsCloudAvailable = false;
                Debug.LogError($"❌ UGS Initialization failed: {e.Message}");
            }
        }
    }
}