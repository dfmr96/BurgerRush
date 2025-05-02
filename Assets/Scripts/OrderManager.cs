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
    
    private void Start()
    {
        // Suscribirse a eventos de expiración de órdenes
        foreach (var order in orders)
        {
            order.OnOrderExpired += HandleOrderExpired;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    [ContextMenu("Generate Order")]
    public void GenerateOrder(BurgerComplexityData complexityData)
    {
        foreach (var order in Orders)
        {
            if (order.gameObject.activeSelf) continue;


            List<IngredientData> selectedIngredients = new();

            selectedIngredients.Add(ingredientsDB.GetRandomIngredientOfType(IngredientType.TopBun));

            int toppingCount = Random.Range(complexityData.MinToppings, complexityData.MaxToppings + 1);
            for (int i = 0; i < toppingCount; i++)
            {
                selectedIngredients.Add(ingredientsDB.GetRandomIngredientOfType(IngredientType.Topping));
            }
            selectedIngredients.Add(ingredientsDB.GetRandomIngredientOfType(IngredientType.Protein));
            selectedIngredients.Add(ingredientsDB.GetRandomIngredientOfType(IngredientType.BottomBun));

            order.SetIngredients(selectedIngredients.ToArray());
            order.SetComplexity(complexityData);
            order.gameObject.SetActive(true);
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
    
    private void HandleOrderExpired(Order expiredOrder)
    {
        Debug.Log("¡Una orden expiró!");

        // Aplicar consecuencias de la orden vencida
        GameManager.Instance.BreakCombo(); // Rompe la racha
        GameManager.Instance.DecreaseScore(50); // Penaliza el puntaje
        // También podrías reproducir un sonido o feedback visual acá
    }
}