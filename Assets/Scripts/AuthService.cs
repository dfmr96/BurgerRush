using System;
using System.Threading.Tasks;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Unity.Services.Authentication;
using UnityEngine;

public class AuthService
{
    public async Task TryAutoLoginGPGS(Action onSuccess, Action<Exception> onFailure)
    {
        if (AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("🙌 Already signed in — skipping auto login.");
            onSuccess?.Invoke();
            return;
        }

        PlayGamesPlatform.Activate();
        var status = await AuthenticateWithGPGS(silent: true);
        await HandleLoginResult(status, onSuccess, onFailure);
    }

    public async Task TryManualLoginGPGS(Action onSuccess, Action<Exception> onFailure)
    {
        if (AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("🙌 Already signed in — skipping manual login.");
            onSuccess?.Invoke();
            return;
        }

        var status = await AuthenticateWithGPGS(silent: false);
        await HandleLoginResult(status, onSuccess, onFailure);
    }

    public async Task SignInAnonymouslyAsync(Action onSuccess, Action<Exception> onFailure)
    {
        if (AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("🙌 Already signed in (anonymous).");
            onSuccess?.Invoke();
            return;
        }

        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"🕶️ Anonymous sign-in. PlayerID: {AuthenticationService.Instance.PlayerId}");
            onSuccess?.Invoke();
        }
        catch (Exception e)
        {
            onFailure?.Invoke(e);
        }
    }

    private async Task HandleLoginResult(SignInStatus status, Action onSuccess, Action<Exception> onFailure)
    {
        if (status == SignInStatus.Canceled)
        {
            Debug.LogWarning("⚠️ GPGS login canceled by user. Show manual login option.");
            onFailure?.Invoke(new OperationCanceledException("GPGS login was canceled."));
            return;
        }

        if (status != SignInStatus.Success)
        {
            onFailure?.Invoke(new Exception($"GPGS SignIn failed: {status}"));
            return;
        }

        try
        {
            string serverAuthCode = await GetServerAuthCodeAsync();

            if (AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.LinkWithGooglePlayGamesAsync(serverAuthCode);
                Debug.Log("🔗 Linked anonymous account with GPGS.");
            }
            else
            {
                await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(serverAuthCode);
                Debug.Log($"✅ Signed in with GPGS. PlayerID: {AuthenticationService.Instance.PlayerId}");
            }

            onSuccess?.Invoke();
        }
        catch (AuthenticationException e) when (e.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
        {
            Debug.LogWarning("⚠️ Account already linked. Retrying sign-in.");
            AuthenticationService.Instance.SignOut();
            await RetrySignInWithGPGS(onSuccess, onFailure);
        }
        catch (Exception e)
        {
            onFailure?.Invoke(e);
        }
    }

    private Task<SignInStatus> AuthenticateWithGPGS(bool silent)
    {
        var tcs = new TaskCompletionSource<SignInStatus>();

        if (silent)
        {
            PlayGamesPlatform.Instance.Authenticate(status => tcs.SetResult(status));
        }
        else
        {
            PlayGamesPlatform.Instance.ManuallyAuthenticate(status => tcs.SetResult(status));
        }

        return tcs.Task;
    }

    private Task<string> GetServerAuthCodeAsync()
    {
        var tcs = new TaskCompletionSource<string>();
        PlayGamesPlatform.Instance.RequestServerSideAccess(true, code => tcs.SetResult(code));
        return tcs.Task;
    }

    private async Task RetrySignInWithGPGS(Action onSuccess, Action<Exception> onFailure)
    {
        try
        {
            string authCode = await GetServerAuthCodeAsync();
            await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(authCode);
            Debug.Log($"✅ Retried sign-in with GPGS. PlayerID: {AuthenticationService.Instance.PlayerId}");
            onSuccess?.Invoke();
        }
        catch (Exception e)
        {
            onFailure?.Invoke(e);
        }
    }
}
