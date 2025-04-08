using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Databases;
using ScriptableObjects;
using ScriptableObjects.BurgerComplexityData;
using UnityEngine;
using Random = UnityEngine.Random;

public class OrderManager : MonoBehaviour
{
    [Tooltip("Reference to the database of ingredients.")] [SerializeField]
    private IngredientsDB ingredientsDB;
    
    [field: Tooltip("Array of orders in Game Scene")]
    
    [SerializeField] private Order[] orders;
    
    public Order[] Orders => orders;

    // ReSharper disable Unity.PerformanceAnalysis
    [ContextMenu("Generate Order")]
    public void GenerateOrder(BurgerComplexityData complexityData)
    {
        foreach (var order in Orders)
        {
            if (order.gameObject.activeSelf) continue;

            order.gameObject.SetActive(true);

            List<IngredientData> selectedIngredients = new();

            selectedIngredients.Add(ingredientsDB.GetRandomIngredientOfType(IngredientType.TopBun));

            int toppingCount = Random.Range(complexityData.minToppings, complexityData.maxToppings + 1);
            for (int i = 0; i < toppingCount; i++)
            {
                selectedIngredients.Add(ingredientsDB.GetRandomIngredientOfType(IngredientType.Topping));
            }
            selectedIngredients.Add(ingredientsDB.GetRandomIngredientOfType(IngredientType.Protein));

            selectedIngredients.Add(ingredientsDB.GetRandomIngredientOfType(IngredientType.BottomBun));
            order.SetIngredients(selectedIngredients.ToArray());
            return;
        }

        Debug.LogWarning("No inactive orders available.");
    }
    
    public int OrdersCountActive()
    {
        int count = 0;
        foreach (var order in orders)
        {
            if (order.gameObject.activeSelf) count++;
        }
        return count;
    }
}