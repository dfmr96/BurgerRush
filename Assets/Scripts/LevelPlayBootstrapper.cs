using System;
using UnityEngine;
using Unity.Services.LevelPlay;

public static class LevelPlayBootstrapper
{
    public static void Initialize(string appKey, Action<LevelPlayConfiguration> onSuccess, Action<LevelPlayInitError> onFail)
    {
        Debug.Log($"🛠️ LevelPlayBootstrapper: Initializing SDK with AppKey: {appKey}");

#if !UNITY_WEBGL
        IronSource.Agent.setAdaptersDebug(true);
        IronSource.Agent.validateIntegration();
#endif

        LevelPlay.OnInitSuccess += HandleInitSuccess;
        LevelPlay.OnInitFailed += HandleInitFailed;

        void HandleInitSuccess(LevelPlayConfiguration config)
        {
            Debug.Log("✅ LevelPlay initialized successfully.");
            LevelPlay.OnInitSuccess -= HandleInitSuccess;
            LevelPlay.OnInitFailed -= HandleInitFailed;
            onSuccess?.Invoke(config);
        }

        void HandleInitFailed(LevelPlayInitError error)
        {
            Debug.LogError("❌ LevelPlay failed to initialize: " + error);
            LevelPlay.OnInitSuccess -= HandleInitSuccess;
            LevelPlay.OnInitFailed -= HandleInitFailed;
            onFail?.Invoke(error);
        }

        LevelPlay.Init(appKey);
    }
}