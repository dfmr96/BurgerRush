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
    
    [field: Tooltip("Array of orders in Game Scene")]
    
    [SerializeField] private Order[] orders;
    
    [Header("Order Difficulty Settings")]
    [SerializeField] private int maxMiddleIngredients = 3;

    public Order[] Orders => orders;

    // ReSharper disable Unity.PerformanceAnalysis
    [ContextMenu("Generate Order")]
    [ContextMenu("Generate Order")]
    public void GenerateOrder()
    {
        foreach (var order in Orders)
        {
            if (order.gameObject.activeSelf) continue;

            order.gameObject.SetActive(true);

            List<IngredientData> selectedIngredients = new();

            var topBun = ingredientsDB.GetRandomIngredientOfType(IngredientType.TopBun);
            selectedIngredients.Add(topBun);
            var bottomBun = ingredientsDB.GetRandomIngredientOfType(IngredientType.BottomBun);

            selectedIngredients.Add(ingredientsDB.GetRandomIngredientOfType(IngredientType.Topping));
            selectedIngredients.Add(ingredientsDB.GetRandomIngredientOfType(IngredientType.Protein));

            selectedIngredients.Add(bottomBun);

            order.SetIngredients(selectedIngredients.ToArray());
            return;
        }

        Debug.LogWarning("No inactive orders available.");
    }
}