using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using ScriptableObjects;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(fileName = "IngredientsDB", menuName = "ScriptableObjects/IngredientsDB", order = 1)]
    public class IngredientsDB : ScriptableObject
    {
        [SerializeField] private IngredientData[] allIngredients;

        [SerializeField]
        private SerializedDictionary<IngredientType, List<IngredientData>> ingredientsByType;

        public IngredientData GetRandomIngredientOfType(IngredientType type)
        {
            EnsureInitialized();

            if (ingredientsByType.TryGetValue(type, out var list) && list.Count > 0)
            {
                int index = Random.Range(0, list.Count);
                return list[index];
            }

            Debug.LogWarning($"No ingredients found for type {type}");
            return null;
        }

        private void EnsureInitialized()
        {
            if (ingredientsByType == null || ingredientsByType.Count == 0)
            {
                ingredientsByType = new SerializedDictionary<IngredientType, List<IngredientData>>();

                foreach (var ingredient in allIngredients)
                {
                    if (!ingredientsByType.ContainsKey(ingredient.Type))
                    {
                        ingredientsByType[ingredient.Type] = new List<IngredientData>();
                    }

                    ingredientsByType[ingredient.Type].Add(ingredient);
                }
            }
        }
    }
}