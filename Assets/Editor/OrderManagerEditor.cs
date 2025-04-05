using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(OrderManager))]
    public class OrderManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);

            // Solo si estamos en Play Mode
            if (Application.isPlaying)
            {
                if (GUILayout.Button("Generate Orders"))
                {
                    OrderManager manager = (OrderManager)target;
                    manager.GenerateOrder();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Enter Play Mode to generate Orders", MessageType.Info);
            }
        }
    }
}