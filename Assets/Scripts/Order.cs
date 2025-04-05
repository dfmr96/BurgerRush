using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using TMPro;
using UnityEngine;

public class Order : MonoBehaviour
{
    [SerializeField] private TMP_Text orderText;
    [field: SerializeField] public IngredientData[] Ingredients { get; private set; }

    public void SetIngredients(IngredientData[] selectedIngredients)
    {
        Ingredients = new IngredientData[selectedIngredients.Length];
        for (int i = 0; i < selectedIngredients.Length; i++)
        {
            Ingredients[i] = selectedIngredients[i];
        }
        
        UpdateText();
    }

    public void UpdateText()
    {
        string order = "Order: ";

        for (int i = 0; i < Ingredients.Length; i++)
        {
            order += $"\n-{Ingredients[i].name}";
        }
        orderText.text = order;
    }
}
