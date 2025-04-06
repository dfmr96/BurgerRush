using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/New Ingredient Data", fileName = "New Ingredient", order = 0)]
    public class IngredientData : ScriptableObject
    {
        [SerializeField] private string ingredientName;
        [SerializeField] private Sprite ingredientIcon;

        public string IngredientName => ingredientName;
        public Sprite IngredientIcon => ingredientIcon;
    }
}