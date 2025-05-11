using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Services.Cloud
{
    public class UGSInitializer : MonoBehaviour
    {
        private async void Awake()
        {
            await InitializeServices();
        }

        private async Task InitializeServices()
        {
            if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
            {
                await UnityServices.InitializeAsync();

                if (!AuthenticationService.Instance.IsSignedIn)
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();

                Debug.Log($"✅ Unity Services initialized. PlayerID: {AuthenticationService.Instance.PlayerId}");
            }
        }
    }
}