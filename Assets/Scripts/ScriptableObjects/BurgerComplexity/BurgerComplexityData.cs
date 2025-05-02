using UnityEngine;

namespace ScriptableObjects.BurgerComplexityData
{
    [CreateAssetMenu(fileName = "BurgerComplexity", menuName = "BurgerRush/Burger Complexity")]
    public class BurgerComplexityData : ScriptableObject
    {
        [Tooltip("Visual identifier only")] 
        [SerializeField] private string label;

        [Tooltip("Min toppings (exclusive of bun and protein)")]
        [SerializeField] private int minToppings;

        [Tooltip("Max toppings (exclusive of bun and protein)")]
        [SerializeField] private int maxToppings;
        
        [SerializeField] private int baseScore;
        [SerializeField] private int pointsPerTopping;
        [SerializeField] private Material scoreTextMaterial;
        public Material ScoreTextMaterial => scoreTextMaterial;
        public string Label => label;
        public int MaxToppings => maxToppings;
        public int MinToppings => minToppings;

        public int BaseScore => baseScore;

        public int PointsPerTopping => pointsPerTopping;
    }
}