using UnityEngine;

namespace ScriptableObjects.BurgerComplexityData
{
    [CreateAssetMenu(fileName = "BurgerComplexity", menuName = "BurgerRush/Burger Complexity")]
    public class BurgerComplexityData : ScriptableObject
    {
        [Tooltip("Visual identifier only")]
        public string label;

        [Tooltip("Min toppings (exclusive of bun and protein)")]
        public int minToppings;

        [Tooltip("Max toppings (exclusive of bun and protein)")]
        public int maxToppings;
    }
}