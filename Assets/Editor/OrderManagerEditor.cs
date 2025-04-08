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

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Enter Play Mode to generate Orders", MessageType.Info);
                return;
            }

            var manager = (OrderManager)target;
            var gameManager = GameManager.Instance;

            if (gameManager == null)
            {
                EditorGUILayout.HelpBox("GameManager not found in the scene.", MessageType.Error);
                return;
            }

            EditorGUILayout.LabelField("Generate Orders by Complexity", EditorStyles.boldLabel);

            var complexities = new[]
            {
                gameManager.EasyBurger,
                gameManager.MediumBurger,
                gameManager.HardBurger
            };

            foreach (var complexity in complexities)
            {
                if (complexity == null) continue;

                if (GUILayout.Button($"Generate: {complexity.label}"))
                {
                    manager.GenerateOrder(complexity);
                }
            }
        }
    }
}