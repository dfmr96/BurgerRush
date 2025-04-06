using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects;
using UnityEngine;

public class IngredientStacker : MonoBehaviour
{
    [SerializeField] private OrderManager orderManager;
    [SerializeField] private Transform stackContainer; // Visual container
    [SerializeField] private GameObject stackedIngredientPrefab; // Prefab for stacked ingredients

    [SerializeField] private float yOffset = 0f;
    private Stack<IngredientData> stackedIngredients = new();
    private const float IngredientHeight = 40f; // Adjust this value based on your prefab height
    private List<GameObject> visualStack = new();
    
    private void Awake()
    {
        if (stackContainer == null)
            Debug.LogError("Stack Parent not assigned in IngredientStacker.");
        if (stackedIngredientPrefab == null)
            Debug.LogError("Stacked Ingredient Prefab not assigned in IngredientStacker.");
    }
    private void Start()
    {
        // Find all IngredientButtons in the scene
        IngredientButton[] buttons = FindObjectsOfType<IngredientButton>();
        foreach (var button in buttons)
        {
            // Initialize each button with the StackIngredient method
            button.Initialize(StackIngredient);
        }
    }

    public void StackIngredient(IngredientData ingredientData)
    {
        if (ingredientData == null)
        {
            Debug.LogWarning("Tried to stack null ingredient");
            return;
        }

        stackedIngredients.Push(ingredientData);
        UpdateStackVisual();
        ValidateOrders();
    }

    public void ClearStack()
    {
        stackedIngredients.Clear();
        UpdateStackVisual();
        ValidateOrders(); // Limpia el highlight
    }
    
    public void TryValidateOrder(Order[] activeOrders)
    {
        foreach (var order in activeOrders)
        {
            if (!order.gameObject.activeSelf) continue;

            var orderIngredients = order.Ingredients;
            if (IsMatch(orderIngredients, stackedIngredients))
            {
                Debug.Log("Order completed!");
                order.Complete();
                ClearStack();
                return;
            }
        }

        Debug.Log("No matching order found.");
    }
    
    private void UpdateStackVisual()
    {
        foreach (var obj in visualStack)
            Destroy(obj);
        visualStack.Clear();

        var stackArray = stackedIngredients.Reverse().ToArray(); // Para mostrar desde abajo hacia arriba
        for (int i = 0; i < stackArray.Length; i++)
        {
            var ingredient = stackArray[i];
            GameObject visual = Instantiate(stackedIngredientPrefab, stackContainer);
            visual.GetComponent<StackedIngredientView>().SetData(ingredient);

            var rect = visual.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, -i * yOffset);

            visualStack.Add(visual);
        }
    }

    private bool IsMatch(Stack<IngredientData> a, Stack<IngredientData> b)
    {
        return a.SequenceEqual(b);
    }

    private void ValidateOrders()
    {
        foreach (var order in orderManager.Orders)
        {
            if (!order.gameObject.activeSelf) continue;

            bool isMatch = IsMatch(order.Ingredients, stackedIngredients);
            order.MarkAsDeliverable(isMatch);
        }
    }
}