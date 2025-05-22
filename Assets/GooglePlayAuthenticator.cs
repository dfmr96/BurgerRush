using System;
using UnityEngine;

public class GooglePlayAuthenticator : MonoBehaviour
{
    public static event Action OnSignedIn;

    private readonly AuthService _authService = new();

    private void Awake()
    {
#if UNITY_EDITOR
        HandleEditorSignIn();
#else
        HandleGPGSSignIn();
#endif
    }

    private void HandleEditorSignIn()
    {
        Debug.Log("🧪 Editor detected — using anonymous sign-in.");
        _ = _authService.SignInAnonymouslyAsync(
            OnAuthSuccess,
            OnEditorAuthFailure
        );
    }

    private void HandleGPGSSignIn()
    {
        _ = _authService.SignInWithGooglePlayAsync(
            OnAuthSuccess,
            OnGPGSAuthFailure
        );
    }

    private void OnAuthSuccess()
    {
        OnSignedIn?.Invoke();
    }

    private void OnEditorAuthFailure(Exception e)
    {
        Debug.LogError($"❌ Anonymous sign-in failed in editor: {e.Message}");
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
}
