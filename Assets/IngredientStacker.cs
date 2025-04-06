using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

public class IngredientStacker : MonoBehaviour
{
    [SerializeField] private Transform stackContainer; // Contenedor visual (la mesa)
    [SerializeField] private GameObject stackedIngredientPrefab; // Prefab visual

    [SerializeField] private float yOffset = 0f;
    [SerializeField] private int currentIndex;
    private const float IngredientHeight = 40f; // Ajustá este valor según tu sprite
    
    private void Awake()
    {
        if (stackContainer == null)
            Debug.LogError("Stack Parent not assigned in IngredientStacker.");
        if (stackedIngredientPrefab == null)
            Debug.LogError("Stacked Ingredient Prefab not assigned in IngredientStacker.");
    }
    private void Start()
    {
        // Encuentra todos los botones de ingredientes en escena
        IngredientButton[] buttons = FindObjectsOfType<IngredientButton>();
        foreach (var button in buttons)
        {
            // Les asigna su callback con el método StackIngredient
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

        var newIngredient = Instantiate(stackedIngredientPrefab, stackContainer);
        newIngredient.GetComponent<StackedIngredientView>().SetData(ingredientData);
        // Posicionar apilado (ajustá según el pivot de los objetos)
        var rectTransform = newIngredient.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, -currentIndex * yOffset);

        currentIndex++;
    }

    public void ClearStack()
    {
        foreach (Transform child in stackContainer)
        {
            Destroy(child.gameObject);
        }
        currentIndex = 0;
    }
}