using System.Collections;
using System.Collections.Generic;
using ScriptableObjects.BurgerComplexityData;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<BurgerComplexityData> BurgerComplexityDatas => burgerComplexityDatas;

    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 60f;
    [SerializeField] private float orderInterval = 5f;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject timeUpPanel;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private Button restartButton;
    [SerializeField] private TMP_Text finalScoreText;

    
    [Header("Score Settings")]
    [SerializeField] private int score = 0;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private int pointsPerOrder = 100;
    [Header("Manager References")]
    [SerializeField] private OrderManager orderManager;

    [Header("Complexity Data")]
    [SerializeField] private List<BurgerComplexityData> burgerComplexityDatas;

    private float timeRemaining;
    private float orderTimer;
    private bool gameRunning = false;
    
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
        timeUpPanel.SetActive(false);
        restartButton.gameObject.SetActive(false);
        StartGame();
    }

    private void Update()
    {
        if (!gameRunning) return;

        timeRemaining -= Time.deltaTime;
        orderTimer += Time.deltaTime;

        UpdateTimerUI();

        if (orderTimer >= orderInterval)
        {
            orderTimer = 0f;
            orderManager.GenerateOrder(BurgerComplexityDatas[0]); //TODO Dynamic difficulty
        }

        if (timeRemaining <= 0f)
        {
            EndGame();
        }
    }

    private void StartGame()
    {
        gameRunning = true;
        timeRemaining = gameDuration;
        orderTimer = 0f;

        timeSlider.maxValue = gameDuration;
        timeSlider.value = gameDuration;
        orderManager.GenerateOrder((BurgerComplexityDatas[0]));
        UpdateTimerUI();
    }

    private void EndGame()
    {
        gameRunning = false;
        timeUpPanel.SetActive(true);
        restartButton.gameObject.SetActive(true);
        
        finalScoreText.text = $"Final Score: {score}";
    }

    private void UpdateTimerUI()
    {
        int seconds = Mathf.CeilToInt(timeRemaining);
        timerText.text = $"Time: {seconds}";

        timeSlider.value = timeRemaining;
    }
    
    public void AddScore()
    {
        score += pointsPerOrder;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = $"Score: {score}";
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
