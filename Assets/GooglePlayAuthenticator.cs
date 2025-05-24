using System;
using UnityEngine;

public class GooglePlayAuthenticator : MonoBehaviour
{
    public static GooglePlayAuthenticator Instance { get; private set; }

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

    public static void ForceRaiseSignedIn() => _onSignedIn?.Invoke();

    private readonly AuthService _authService = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("🕹️ GooglePlayAuthenticator: Awake. Waiting for UGS to be initialized.");
        // No hagas nada más aquí
    }

    public void TryAutoLogin()
    {
        Debug.Log("🚀 Attempting GPGS auto login...");
        _authService.TryAutoLoginGPGS(
            OnAuthSuccess,
            OnGPGSAuthFailure
        );
    }

    public void RetryManualSignIn()
    {
        Debug.Log("🔄 Retrying GPGS with manual login popup...");
        _authService.TryManualLoginGPGS(
            OnAuthSuccess,
            OnGPGSAuthFailure
        );
    }

    private async void OnGPGSAuthFailure(Exception e)
    {
        if (e is OperationCanceledException)
        {
            Debug.LogWarning("⚠️ GPGS login was canceled — waiting for manual retry.");
            // Mostrás botón desde la UI (no hacemos fallback aquí)
            return;
        }

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

    private void OnAuthSuccess()
    {
        Debug.Log("✅ GooglePlayAuthenticator: OnAuthSuccess()");
        _onSignedIn?.Invoke();
    }
}
