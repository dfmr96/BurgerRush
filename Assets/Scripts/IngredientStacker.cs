using System.Collections.Generic;
using System.Linq;
using Enums;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

public class IngredientStacker : MonoBehaviour
{
    [SerializeField] private OrderManager orderManager;
    [SerializeField] private Transform stackContainer; // Visual container
    [SerializeField] private GameObject stackedIngredientPrefab; // Prefab for stacked ingredients
    [SerializeField] private List<IngredientButton> ingredientButtons;
    [SerializeField] private Button  deliverButton;

    [SerializeField] private float yOffset;
    private readonly Stack<IngredientData> _stackedIngredients = new();
    private readonly List<GameObject> _visualStack = new();

    public OrderManager OrderManager => orderManager;

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
        foreach (var button in ingredientButtons)
            // Initialize each button with the StackIngredient method
            button.Initialize(StackIngredient);
    }

    private void StackIngredient(IngredientData ingredientData)
    {
        if (ingredientData == null)
        {
            Debug.LogWarning("Tried to stack null ingredient");
            return;
        }

        _stackedIngredients.Push(ingredientData);
        PlayerStatsManager.AddIngredient();
        UpdateStackVisual();
        ValidateOrders();
        
        AudioManager.Instance?.PlaySFX(SFXType.IngredientPlaced);

    }

    public void ClearStack()
    {
        _stackedIngredients.Clear();
        PlayerStatsManager.AddDiscarded();
        UpdateStackVisual();
        ValidateOrders();
        
        AudioManager.Instance?.PlaySFX(SFXType.StackCleared);
    }
    
    private void UpdateStackVisual()
    {
        foreach (var obj in _visualStack)
            Destroy(obj);
        _visualStack.Clear();

        var stackArray = _stackedIngredients.Reverse().ToArray(); // Para mostrar desde abajo hacia arriba
        for (var i = 0; i < stackArray.Length; i++)
        {
            var ingredient = stackArray[i];
            var visual = Instantiate(stackedIngredientPrefab, stackContainer);
            visual.GetComponent<StackedIngredientView>().SetData(ingredient);

            var rect = visual.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, -i * yOffset);

            _visualStack.Add(visual);
        }
    }
    private bool IsMatch(Stack<IngredientData> a, Stack<IngredientData> b)
    {
        return a.SequenceEqual(b);
    }

    private void ValidateOrders()
    {
        bool hasValidOrder = false;

        foreach (var order in OrderManager.Orders)
        {
            if (!order.gameObject.activeSelf) continue;

            var isMatch = IsMatch(order.Ingredients, _stackedIngredients);
            order.MarkAsDeliverable(isMatch);

            if (isMatch)
                hasValidOrder = true;
        }

        deliverButton.interactable = hasValidOrder;
    }

    public void ValidateOrder(Order order)
    {
        var isMatch = IsMatch(order.Ingredients, _stackedIngredients);
        order.MarkAsDeliverable(isMatch);
    }
    public void OnClearStackButtonPressed()
    {
        ClearStack();
    }
}