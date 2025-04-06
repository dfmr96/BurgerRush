using System;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

public class IngredientButton : MonoBehaviour
{
    [SerializeField] private IngredientData ingredientData;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;

    public IngredientData Ingredient => ingredientData;

    private void Start()
    {
        if (ingredientData == null)
        {
            Debug.LogWarning($"IngredientData not assigned in {gameObject.name}");
        }
    }

    public void Initialize(Action<IngredientData> onClickCallback)
    {
        iconImage.sprite = ingredientData.IngredientIcon;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClickCallback?.Invoke(ingredientData));
    }
    
    private void OnIngredientClicked(IngredientData ingredient)
    {
        Debug.Log($"Clicked ingredient: {ingredient.name}");
        // Handle the ingredient click event here
    }
}
