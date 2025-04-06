using System;
using System.Collections;
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
    [field: SerializeField] public Stack<IngredientData> Ingredients { get; private set; } = new();
    private bool isDeliverable = false;


    private void Start()
    {
        deliverButton.onClick.AddListener(AttemptDelivery);
    }

    public void SetIngredients(IngredientData[] ingredients)
    {
        Ingredients = new Stack<IngredientData>();

        for (int i = ingredients.Length - 1; i >= 0; i--)
        {
            Ingredients.Push(ingredients[i]);
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        string order = "Order:";
        foreach (var ingredient in Ingredients) // Mostrar de abajo hacia arriba
        {
            order += $"\n- {ingredient.IngredientName}";
        }
        orderText.text = order;
    }

    public void MarkAsDeliverable(bool canDeliver)
    {
        isDeliverable = canDeliver;
        highlightImage.enabled = canDeliver;

        if (canDeliver)
        {
            Debug.Log($"Orden {name} se puede entregar ✅");
        }
    }

    private void AttemptDelivery()
    {
        if (isDeliverable)
        {
            Complete();
            stacker.ClearStack(); // Resetea la pila al entregar
        }
        else
        {
            Debug.Log("This order does not match the current stack.");
        }
    }
    
    public void Complete()
    {
        Debug.Log("Order delivered!");
        highlightImage.enabled = false;
        gameObject.SetActive(false);
        // Hacés lo que necesites para completar la orden (animación, sonido, etc)
    }
}
