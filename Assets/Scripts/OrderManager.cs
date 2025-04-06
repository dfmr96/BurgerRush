using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using Databases;
using ScriptableObjects;
using Random = UnityEngine.Random;

public class OrderManager : MonoBehaviour
{
    [field: Tooltip("Array of orders in Game Scene")]
    public Order[] Orders;

    [Tooltip("Reference to the database of ingredients.")] [SerializeField]
    private IngredientsDB ingredientsDB;

    [Tooltip("Dictionary to cache ingredients by name for faster lookup.")] [SerializeField]
    private SerializedDictionary<string, IngredientData> ingredientLookup;

    [SerializeField] private SerializedDictionary<IngredientData, int> ingredientIndexMap;

    private void Awake()
    {
        // Cache ingredients by name and index for faster lookup
        ingredientLookup = new SerializedDictionary<string, IngredientData>(ingredientsDB.ingredients.Length);
        ingredientIndexMap = new SerializedDictionary<IngredientData, int>(ingredientsDB.ingredients.Length);

        // Initialize the dictionaries
        for (int i = 0; i < ingredientsDB.ingredients.Length; i++)
        {
            var ingredient = ingredientsDB.ingredients[i];
            ingredientLookup[ingredient.name] = ingredient;
            ingredientIndexMap[ingredient] = i;
        }
    }
    
    [ContextMenu("Generate Order")]
    public void GenerateOrder()
    {
        foreach (var order in Orders)
        {
            if (order.gameObject.activeSelf) continue;

            order.gameObject.SetActive(true);

            int maxIngredientCount = ingredientsDB.ingredients.Length;
            int maxMiddleIngredients = maxIngredientCount - 2;

            int ingredientCount = Random.Range(2, maxMiddleIngredients + 2); // +2 para incluir los panes
            var selectedIngredients = new IngredientData[ingredientCount];

            if (!TryGetBreadIngredients(out var upperBread, out var lowerBread))
            {
                return;
            }

            selectedIngredients[0] = upperBread;
            selectedIngredients[ingredientCount - 1] = lowerBread;

            // Initialize used indices with the bread indices
            var usedIndices = new HashSet<int>
            {
                ingredientIndexMap[upperBread],
                ingredientIndexMap[lowerBread]
            };

            for (int i = 1; i < ingredientCount - 1; i++)
            {
                var ingredient = GetRandomAvailableIngredient(usedIndices);
                if (ingredient == null)
                {
                    Debug.LogError("Failed to get a unique ingredient. Aborting order generation.");
                    return;
                }

                selectedIngredients[i] = ingredient;
            }

            order.SetIngredients(selectedIngredients);
            return;
        }

        Debug.LogWarning("No inactive orders available.");
    }

    private IngredientData GetRandomAvailableIngredient(HashSet<int> usedIndices)
    {
        var totalIngredients = ingredientsDB.ingredients;
        int maxAttempts = 10;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            int randomIndex = Random.Range(0, totalIngredients.Length);
            if (!usedIndices.Contains(randomIndex))
            {
                usedIndices.Add(randomIndex);
                return totalIngredients[randomIndex];
            }
        }

        Debug.LogWarning("No more available unique ingredients after multiple attempts.");
        return null;
    }

    private bool TryGetBreadIngredients(out IngredientData upperBread, out IngredientData lowerBread)
    {
        bool foundUpper = ingredientLookup.TryGetValue("UpperBread", out upperBread);
        bool foundLower = ingredientLookup.TryGetValue("LowerBread", out lowerBread);

        if (!foundUpper || !foundLower)
        {
            Debug.LogError("Error: Missing required bread ingredients in the database.");
            return false;
        }

        return true;
    }
}