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

        private async void Awake()
        {
            await InitializeServices();
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

                Debug.Log($"✅ Unity Services initialized. PlayerID: {AuthenticationService.Instance.PlayerId}");
                OnUGSReady?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ UGS Initialization failed: {e.Message}");
            }
        }
    }
}