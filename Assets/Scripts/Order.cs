using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using TMPro;
using UnityEngine;

public class Order : MonoBehaviour
{
    [SerializeField] private TMP_Text orderText;
    [SerializeField] private IngredientData[] ingredients;
    
    private void Start()
    {
        string order = "Order: ";

        for (int i = 0; i < ingredients.Length; i++)
        {
            order += $"\n-{ingredients[i].name}";
        }
        orderText.text = order;
    }
}
