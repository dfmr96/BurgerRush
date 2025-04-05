using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/New Ingredient Data", fileName = "New Ingredient", order = 0)]
    public class IngredientData : ScriptableObject
    {
        [SerializeField] private string ingredientName; // Name of the ingredient
        [SerializeField] private Sprite ingredientIcon; // Icon of the ingredient
    }
}
