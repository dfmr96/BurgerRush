using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/New Ingredient Data", fileName = "New Ingredient", order = 0)]
    public class IngredientData : ScriptableObject
    {
        [SerializeField] private string ingredientName;
        [SerializeField] private Sprite ingredientIcon;
        [SerializeField] private IngredientType type;

        public string IngredientName => ingredientName;
        public Sprite IngredientIcon => ingredientIcon;
        public IngredientType Type => type;

    }
}