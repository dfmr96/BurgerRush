#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class EDMForceMenu
{
    [MenuItem("Tools/EDM4U/Force VersionHandler Settings")]
    public static void ForceOpenSettings()
    {
        EditorApplication.ExecuteMenuItem("Assets/External Dependency Manager/Version Handler/Settings");
    }
}
#endif