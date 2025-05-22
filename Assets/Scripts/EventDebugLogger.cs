using System;
using UnityEngine;

public static class EventDebugLogger
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public static void LogSubscribe(Delegate del, string eventName)
    {
        if (del == null) return;

        var method = del.Method;
        var target = del.Target;
        string className = method.DeclaringType?.Name ?? "UnknownClass";
        string methodName = method.Name;
        string targetName = target?.ToString() ?? "static";

        Debug.Log($"🟢 [{eventName}] Subscribed: {className}.{methodName} [Target: {targetName}]");
    }

    public static void LogUnsubscribe(Delegate del, string eventName)
    {
        if (del == null) return;

        var method = del.Method;
        string className = method.DeclaringType?.Name ?? "UnknownClass";
        string methodName = method.Name;

        Debug.Log($"🔴 [{eventName}] Unsubscribed: {className}.{methodName}");
    }
#endif
}