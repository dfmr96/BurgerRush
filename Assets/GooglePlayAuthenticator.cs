using System;
using UnityEngine;

public class GooglePlayAuthenticator : MonoBehaviour
{
    private static event Action _onSignedIn;
    public static event Action OnSignedIn
    {
        add
        {
            _onSignedIn += value;
            EventDebugLogger.LogSubscribe(value, "OnSignedIn");
        }
        remove
        {
            _onSignedIn -= value;
            EventDebugLogger.LogUnsubscribe(value, "OnSignedIn");
        }
    }

    public static void ForceRaiseSignedIn()
    {
        _onSignedIn?.Invoke();
    }

    private readonly AuthService _authService = new();

    private void Awake()
    {
#if !UNITY_EDITOR
        HandleGPGSSignIn();
#else
        Debug.Log("🧪 Editor detected — waiting for UGSInitializer to handle anonymous sign-in.");
#endif
    }

#if !UNITY_EDITOR
    private void HandleGPGSSignIn()
    {
        _ = _authService.SignInWithGooglePlayAsync(
            OnAuthSuccess,
            OnGPGSAuthFailure
        );
    }

    private async void OnGPGSAuthFailure(Exception e)
    {
        Debug.LogWarning($"❌ GPGS sign-in failed: {e.Message}");
        await _authService.SignInAnonymouslyAsync(
            OnAuthSuccess,
            OnAnonymousFallbackFailure
        );
    }

    private void OnAnonymousFallbackFailure(Exception e)
    {
        Debug.LogError($"❌ Anonymous fallback failed: {e.Message}");
    }
#endif

    private void OnAuthSuccess()
    {
        _onSignedIn?.Invoke();
    }
}