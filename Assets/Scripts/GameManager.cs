using System.Collections;
using System.Collections.Generic;
using Databases;
using Enums;
using ScriptableObjects;
using ScriptableObjects.BurgerComplexity;
using Services.Cloud;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private PlayerStatsDatabase statsDB;

    [Header("Debug Info")] 
    [SerializeField, ReadOnly] private string currentDifficultyLabel;
    [SerializeField, ReadOnly] private int currentProgressionIndex = -1;
    [SerializeField, ReadOnly] private int deliveredOrders = 0;

    [Header("Game Settings")] [SerializeField]
    private float gameDuration = 60f;
    
    [Header("UI Elements")] [SerializeField]
    private TMP_Text timerText;

    [SerializeField] private GameObject timeUpPanel;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private TMP_Text finalScoreText;


    [Header("Score Settings")] 
    [SerializeField] private int score = 0;
    [SerializeField] private TMP_Text scoreText;

    [Header("Manager References")] 
    [SerializeField] private OrderManager orderManager;

    [Header("Complexity Data")] 
    [SerializeField] private BurgerComplexityData easyBurger;
    [SerializeField] private BurgerComplexityData mediumBurger;
    [SerializeField] private BurgerComplexityData hardBurger;
    [SerializeField] private List<ComplexityProgressionStep> progression;

    [Header("Order Slot Settings")] 
    [SerializeField] private int maxConcurrentOrders = 3;
    [SerializeField] private float minOrderDelay = 1f;
    [SerializeField] private float maxOrderDelay = 2.5f;

    
    [SerializeField] private ScorePopUp scorePopup;
    [SerializeField] private ComboManager comboManager;
    [SerializeField] private SFXType gameplayTheme;
    [SerializeField] private float speedUpThreshold;

    private float timeRemaining;
    private bool musicSpeedUp = false;
    private bool isGameRunning = false;
    private bool isSpawningOrder = false;
    private float totalSessionTime = 0f;
    public BurgerComplexityData EasyBurger => easyBurger;
    public BurgerComplexityData MediumBurger => mediumBurger;
    public BurgerComplexityData HardBurger => hardBurger;
    public bool IsGameRunning => isGameRunning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        isGameRunning = true;
        StartGame();
    }

    private void Update()
    {
        if (!IsGameRunning) return;
        
        totalSessionTime += Time.deltaTime;

        timeRemaining -= Time.deltaTime;
        UpdateTimerUI();

        if (timeRemaining <= 0f)
        {
            EndGame();
            return;
        }

        if (!isSpawningOrder)
        {
            int activeOrders = orderManager.OrdersCountActive();
            if (activeOrders < maxConcurrentOrders)
            {
                StartCoroutine(GenerateOrderWithDelay());
            }
        }

        HandleMusicSpeedUp();
    }

    private void HandleMusicSpeedUp()
    {
        if (timeRemaining <= speedUpThreshold && !musicSpeedUp)
        {
            AudioManager.Instance.SetMusicPitch(1.25f);
            musicSpeedUp = true;
        }
        else if (timeRemaining > speedUpThreshold && musicSpeedUp)
        {
            AudioManager.Instance.SetMusicPitch(1f);
            musicSpeedUp = false;
        }
    }

    private void AddBonusTime(float amount)
    {
        timeRemaining += amount;
        timeRemaining = Mathf.Min(timeRemaining, gameDuration); // Evita pasar el tiempo máximo

        UpdateTimerUI(); // Refresca UI si es necesario
        Debug.Log($"🕒 Tiempo bonus añadido: +{amount} segundos");
    }

    private void ShowScorePopup(int _score, BurgerComplexityData complexity)
    {
        scorePopup.Show(_score, complexity);
    }


    private void StartGame()
    {
        PlayerStatsManager.AddPlay();
        AudioManager.Instance.PlayBackgroundMusic(gameplayTheme);
        AudioManager.Instance.SetMusicPitch(1f);
        timeUpPanel.SetActive(false);

        isGameRunning = true;
        timeRemaining = gameDuration;
        timeSlider.maxValue = gameDuration;
        timeSlider.value = gameDuration;
        UpdateTimerUI();
        orderManager.GenerateOrder(easyBurger);
    }

    private async void EndGame()
    {
        StopAllCoroutines();
        AudioManager.Instance.PlayBackgroundMusic(SFXType.GameOverTheme);
        isGameRunning = false;
        timeUpPanel.SetActive(true);

        int secondsPlayed = Mathf.FloorToInt(totalSessionTime);
        PlayerStatsManager.AddSecondsPlayed(secondsPlayed);
        PlayerStatsManager.UpdateLongestSession(secondsPlayed);
        PlayerStatsManager.UpdateHighScore(score);
        
        finalScoreText.text = 
            $"<color=#FFD700><b>Final Score:</b></color> {score}\n" +
            $"<color=#00FFFF><b>Best Score:</b></color> {PlayerStatsManager.GetHighScore()}";
        
        await CloudSaveStatsHandler.SaveStatsToCloud(statsDB);
    }

    private void UpdateTimerUI()
    {
        int seconds = Mathf.CeilToInt(timeRemaining);
        timerText.text = $"Time: {seconds}";

        timeSlider.value = timeRemaining;
    }

    private void UpdateScoreUI()
    {
        scoreText.text = $"Score: {score}";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void BreakCombo()
    {
        comboManager.ResetCombo();
        Debug.Log("Combo broken!!!");
    }

    public void DecreaseScore(int amount)
    {
        score = Mathf.Max(0, score - amount); // Evita puntaje negativo
        UpdateScoreUI();
        Debug.Log($"Puntaje penalizado: -{amount}. Puntaje actual: {score}");
    }

    public void OnOrderDelivered(BurgerComplexityData complexity, Stack<IngredientData> ingredients, bool gaveBonus)
    {
        PlayerStatsManager.AddBurger();
        
        switch (complexity.Difficulty)
        {
            case BurgerComplexityData.DifficultyType.Easy:
                PlayerStatsManager.AddEasyDelivery();
                break;
            case BurgerComplexityData.DifficultyType.Medium:
                PlayerStatsManager.AddMediumDelivery();
                break;
            case BurgerComplexityData.DifficultyType.Hard:
                PlayerStatsManager.AddHardDelivery();
                break;
        }
        
        comboManager.RegisterDelivery();
        
        int baseScore = CalculateScore(complexity, ingredients);
        float multiplier = comboManager.GetMultiplier();
        int finalScore = Mathf.RoundToInt(baseScore * multiplier);
        score += finalScore;
        
        if (gaveBonus && complexity.BonusTimeOnDelivery > 0f)
        {
            AddBonusTime(complexity.BonusTimeOnDelivery);
        }

        deliveredOrders++;
        UpdateScoreUI();
        ShowScorePopup(finalScore, complexity);
    }
    private int CalculateScore(BurgerComplexityData complexity, Stack<IngredientData> ingredients)
    {
        return complexity.BaseScore + CountToppings(ingredients) * complexity.PointsPerTopping;
    }

    private static int CountToppings(Stack<IngredientData> ingredients)
    {
        int toppingCount = 0;
        foreach (var ingredient in ingredients)
        {
            if (ingredient.Type == IngredientType.Topping)
            {
                toppingCount++;
            }
        }

        return toppingCount;
    }

    private ComplexityProgressionStep GetCurrentProgressionStep()
    {
        for (int i = 0; i < progression.Count; i++)
        {
            var step = progression[i];
            if (deliveredOrders >= step.minOrdersDelivered && deliveredOrders <= step.maxOrdersDelivered)
            {
                currentProgressionIndex = i;
                currentDifficultyLabel =
                    $"Step {currentProgressionIndex} ({step.minOrdersDelivered}-{step.maxOrdersDelivered})";
                return step;
            }
        }

        return null;
    }

    private IEnumerator GenerateOrderWithDelay()
    {
        isSpawningOrder = true;

        float delay = Random.Range(minOrderDelay, maxOrderDelay);
        yield return new WaitForSeconds(delay);

        var step = GetCurrentProgressionStep();
        if (step == null)
        {
            Debug.LogWarning("No complexity step found.");
            isSpawningOrder = false;
            yield break;
        }

        var complexity = step.GetRandomComplexity(easyBurger, mediumBurger, hardBurger);
        orderManager.GenerateOrder(complexity);
        isSpawningOrder = false;
    }
}