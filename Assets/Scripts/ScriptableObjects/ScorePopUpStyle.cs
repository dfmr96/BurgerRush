using TMPro;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewScoreStyle", menuName = "BurgerRush/Score Popup Style")]

    public class ScorePopUpStyle : ScriptableObject
    {
        public Material fontMaterial;
        public Color gradientTopLeft = Color.white;
        public Color gradientTopRight = Color.white;
        public Color gradientBottomLeft = Color.white;
        public Color gradientBottomRight = Color.white;

        public VertexGradient Gradient =>
            new VertexGradient(gradientTopLeft, gradientTopRight, gradientBottomLeft, gradientBottomRight);
    }
}
