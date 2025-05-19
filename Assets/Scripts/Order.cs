using System;
using System.Collections.Generic;
using Enums;
using ScriptableObjects;
using ScriptableObjects.BurgerComplexity;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Order : MonoBehaviour
{
    [SerializeField] private Button deliverButton;
    [SerializeField] private Image highlightImage;
    [SerializeField] private IngredientStacker stacker;
    [SerializeField] private BurgerComplexityData complexity;
    [SerializeField] private float lifespan = 10f; // segundos
    [SerializeField] private Image timerBar;
    [SerializeField] private Slider timerSlider;
    [SerializeField] private Transform ingredientContainer; // contenedor con Vertical Layout
    [SerializeField] private Image ingredientContainerImage;
    [SerializeField] private GameObject ingredientImagePrefab; // prefab con Image
    [SerializeField] private RectTransform orderRectTransform;
    [SerializeField] private GameObject bonusIcon;


    
    public Stack<IngredientData> Ingredients { get; private set; } = new();
    public Action<Order> OnOrderExpired;
    public bool IsBonusOrder => isBonusOrder;

    public BurgerComplexityData Complexity => complexity;

    private bool _isDeliverable;
    private bool isBonusOrder = false;
    private float timer;
    private bool isExpired;

    
    private void OnEnable()
    {
        timer = lifespan;
        isExpired = false;
        stacker.ValidateOrder(this);
        if (timerBar != null)
            timerBar.fillAmount = 1f;

        if (bonusIcon != null)
            bonusIcon.SetActive(false);
        
        if (complexity != null && orderRectTransform != null)
        {
            AudioManager.Instance.PlayNewOrderSFX(complexity, orderRectTransform);
        }
    }
    
    private void Start()
    {
        deliverButton.onClick.AddListener(AttemptDelivery);
    }
    
    private void Update()
    {
        if (!GameManager.Instance || !GameManager.Instance.IsGameRunning) return;
        if (isExpired) return;

        timer -= Time.deltaTime;

        if (timerBar != null)
        {
            float normalizedTime = timer / lifespan;

            // Update fill amount
            timerSlider.value = normalizedTime;

            // Base color: verde a rojo
            Color baseColor = Color.Lerp(Color.red, Color.green, normalizedTime);

            // Si quedan menos de 3 segundos, hacer que el color parpadee (oscile su intensidad)
            if (timer <= 3f)
            {
                float pulse = Mathf.PingPong(Time.time * 4f, 1f); // velocidad de parpadeo
                baseColor = Color.Lerp(Color.red * 0.5f, Color.red, pulse);
            }

            timerBar.color = baseColor;
        }

        if (timer <= 0f)
        {
            ExpireOrder();
        }
    }
    
    public void SetBonus(bool value)
    {
        isBonusOrder = value;

        if (bonusIcon != null)
            bonusIcon.SetActive(isBonusOrder);
    }
    
    private void ExpireOrder()
    {
        if (isExpired) return;
        isExpired = true;
        
        AudioManager.Instance?.PlaySFX(SFXType.OrderExpired);

        OnOrderExpired?.Invoke(this);
        gameObject.SetActive(false);
    }

    public void SetIngredients(IngredientData[] ingredients)
    {
        Ingredients = new Stack<IngredientData>();
        for (var i = ingredients.Length - 1; i >= 0; i--) Ingredients.Push(ingredients[i]);
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Limpiar sprites anteriores
        foreach (Transform child in ingredientContainer)
        {
            Destroy(child.gameObject);
        }

        var ingredientList = new List<IngredientData>(Ingredients);

        // Mostrar ingredientes de abajo hacia arriba, los últimos deben tener mayor prioridad
        for (int i = 0; i < ingredientList.Count; i++)
        {
            var imageGO = Instantiate(ingredientImagePrefab, ingredientContainer);
            var image = imageGO.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = ingredientList[i].IngredientIcon;
            }

            // Este ingrediente se coloca al fondo visual (para que el siguiente lo tape)
            imageGO.transform.SetAsFirstSibling();
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(orderRectTransform);
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
    public void AddBonusTime(float bonus)
    {
        timer += bonus;
        timer = Mathf.Min(timer, lifespan); // evita pasarse del máximo

        // Opcional: feedback visual
        Debug.Log($"🕒 Orden extendida: +{bonus}s restantes");
    }

    private void Complete()
    {
        GameManager.Instance.OnOrderDelivered(Complexity, Ingredients, isBonusOrder);

        if (timer <= 3f && timer > 0f)
        {
            PlayerStatsManager.AddClutchDelivery();
            AudioManager.Instance?.PlaySFX(SFXType.ClutchDelivery);
        }
        else
        {
            AudioManager.Instance.PlayOrderDeliveredSFX(ComboManager.Instance.CurrentStreak);
        }
        VibrationManager.Vibrate(VibrationPreset.OrderCompleted);
        stacker.OrderManager.AddTimeToAllActiveOrdersExcept(this, complexity.ClutchBonusTime);
        
        // Tracking burger_built duration
        if (stacker != null)
        {
            float? startTime = stacker.GetStackStartTime();
            if (startTime.HasValue)
            {
                float duration = Time.time - startTime.Value;
                int ingredientCount = Ingredients.Count;
                string difficulty = complexity.Difficulty.ToString();

                AnalyticsManager.TrackBurgerBuilt(duration, ingredientCount, difficulty);
            }
        }

        highlightImage.enabled = false;
        gameObject.SetActive(false);
    }
    public void SetComplexity(BurgerComplexityData data)
    {
        complexity = data;
        InitBurger(data);
    }

    private void InitBurger(BurgerComplexityData data)
    {
        ingredientContainerImage.color = data.OrderBackgroundColor;
        lifespan = data.ExpirationTime;
    }
}