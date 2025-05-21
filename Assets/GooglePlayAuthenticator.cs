using System;
using System.Threading.Tasks;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Unity.Services.Authentication;
using UnityEngine;

public class GooglePlayAuthenticator : MonoBehaviour
{
    public static event Action OnSignedIn;
    public static bool IsGPGSSignedIn { get; private set; }

    private void Awake()
    {
        TrySignInWithGooglePlayGames();
    }

    private void TrySignInWithGooglePlayGames()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    private void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            IsGPGSSignedIn = true;
            Debug.Log("🔓 GPGS login successful.");

            PlayGamesPlatform.Instance.RequestServerSideAccess(true, async code =>
            {
                Debug.Log("📨 GPGS Server Auth Code obtained.");
                await SignInOrLinkWithUGS(code);
            });
        }
        else
        {
            Debug.LogWarning($"❌ GPGS login failed: {status}");
            _ = SignInAnonymouslyFallback();
        }
    }

    private async Task SignInOrLinkWithUGS(string authCode)
    {
        try
        {
            if (AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.LinkWithGooglePlayGamesAsync(authCode);
                Debug.Log("🔗 Linked anonymous account with GPGS.");
            }
            else
            {
                await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(authCode);
                Debug.Log($"✅ Signed in with GPGS. PlayerID: {AuthenticationService.Instance.PlayerId}");
            }

            OnSignedIn?.Invoke();
        }
        catch (AuthenticationException e) when (e.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
        {
            Debug.LogWarning("⚠️ Account already linked, signing in instead.");
            await SignInWithGPGS(authCode);
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error linking/signing with GPGS: {e.Message}");
            await SignInAnonymouslyFallback();
        }
    }

    private async Task SignInWithGPGS(string authCode)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(authCode);
            Debug.Log($"✅ Signed in with GPGS. PlayerID: {AuthenticationService.Instance.PlayerId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Failed to sign in with GPGS: {e.Message}");
            await SignInAnonymouslyFallback();
        }
    }

    private async Task SignInAnonymouslyFallback()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"🕶️ Fallback anonymous sign-in. PlayerID: {AuthenticationService.Instance.PlayerId}");
                OnSignedIn?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Anonymous fallback failed: {e.Message}");
            }
        }
    }
}
