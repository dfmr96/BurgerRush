using System.Collections.Generic;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Order : MonoBehaviour
{
    [SerializeField] private Button deliverButton;
    [SerializeField] private Image highlightImage;
    [SerializeField] private TMP_Text orderText;
    [SerializeField] private IngredientStacker stacker;
    [SerializeField] private GameManager gameManager;

    public Stack<IngredientData> Ingredients { get; private set; } = new();
    private bool _isDeliverable;

    private void Start()
    {
        deliverButton.onClick.AddListener(AttemptDelivery);
    }

    public void SetIngredients(IngredientData[] ingredients)
    {
        Ingredients = new Stack<IngredientData>();
        for (var i = ingredients.Length - 1; i >= 0; i--) Ingredients.Push(ingredients[i]);
        UpdateUI();
    }

    private void UpdateUI()
    {
        var order = "Order:";
        foreach (var ingredient in Ingredients) order += $"\n- {ingredient.IngredientName}";
        orderText.text = order;
    }

    public void MarkAsDeliverable(bool canDeliver)
    {
        _isDeliverable = canDeliver;
        highlightImage.enabled = canDeliver;

        if (canDeliver) Debug.Log($"Order is deliverable: {string.Join(", ", Ingredients)}");
    }

    private void AttemptDelivery()
    {
        if (_isDeliverable)
        {
            Complete();
            stacker.ClearStack();
        }
        else
        {
            Debug.Log("This order does not match the current stack.");
        }
    }

    private void Complete()
    {
        Debug.Log("Order delivered!");
        gameManager.AddScore();
        highlightImage.enabled = false;
        gameObject.SetActive(false);
    }
}