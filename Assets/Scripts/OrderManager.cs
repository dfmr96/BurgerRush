using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Databases;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

public class OrderManager : MonoBehaviour
{
    [Tooltip("Reference to the database of ingredients.")] [SerializeField]
    private IngredientsDB ingredientsDB;
    
    [SerializeField] private SerializedDictionary<IngredientData, int> ingredientIndexMap;

    [field: Tooltip("Array of orders in Game Scene")]
    
    [SerializeField] private Order[] orders;
    
    [SerializeField] private IngredientData topBun;
    [SerializeField] private IngredientData bottomBun;
    
    [Header("Order Difficulty Settings")]
    [SerializeField] private int maxMiddleIngredients = 3;

    public Order[] Orders => orders;

    private void Awake()
    {
        // Cache ingredients by name and index for faster lookup
        ingredientIndexMap = new SerializedDictionary<IngredientData, int>(ingredientsDB.ingredients.Length);

        // Initialize the dictionaries
        for (var i = 0; i < ingredientsDB.ingredients.Length; i++)
        {
            var ingredient = ingredientsDB.ingredients[i];
            ingredientIndexMap[ingredient] = i;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    [ContextMenu("Generate Order")]
    public void GenerateOrder()
    {
        foreach (var order in Orders)
        {
            if (order.gameObject.activeSelf) continue;

            order.gameObject.SetActive(true);

            int maxAvailableMiddle = ingredientsDB.ingredients.Length - 2;
            int clampedMiddleCount = Mathf.Clamp(maxMiddleIngredients, 0, maxAvailableMiddle);

            int ingredientCount = Random.Range(2, clampedMiddleCount + 2); // +2 for buns
            var selectedIngredients = new IngredientData[ingredientCount];

            if (!TryGetBreadIngredients(out var upperBread, out var lowerBread)) return;

            selectedIngredients[0] = upperBread;
            selectedIngredients[ingredientCount - 1] = lowerBread;

            // Initialize used indices with the bread indices
            var usedIndices = new HashSet<int>
            {
                ingredientIndexMap[upperBread],
                ingredientIndexMap[lowerBread]
            };

            for (var i = 1; i < ingredientCount - 1; i++)
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
        const int maxAttempts = 10;

        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            var randomIndex = Random.Range(0, totalIngredients.Length);
            if (usedIndices.Add(randomIndex))
            {
                return totalIngredients[randomIndex];
            }
        }

        Debug.LogWarning("No more available unique ingredients after multiple attempts.");
        return null;
    }

    private bool TryGetBreadIngredients(out IngredientData upperBread, out IngredientData lowerBread)
    {
        upperBread = topBun;
        lowerBread = bottomBun;

        if (upperBread == null || lowerBread == null)
        {
            Debug.LogError("Top or Bottom Bun references not assigned in the Inspector.");
            return false;
        }

        if (!ingredientIndexMap.ContainsKey(upperBread) || !ingredientIndexMap.ContainsKey(lowerBread))
        {
            Debug.LogError("Top or Bottom Bun not found in ingredient database.");
            return false;
        }

        return true;
    }
}