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
    [field: SerializeField] public IngredientData[] Ingredients { get; private set; }
    private bool isDeliverable = false;


    private void Start()
    {
        deliverButton.onClick.AddListener(AttemptDelivery);
    }

    public void SetIngredients(IngredientData[] ingredients)
    {
        Ingredients = ingredients;

        UpdateUI();
    }

    public void UpdateUI()
    {
        string order = "Order: ";

        for (int i = 0; i < Ingredients.Length; i++)
        {
            order += $"\n-{Ingredients[i].name}";
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
