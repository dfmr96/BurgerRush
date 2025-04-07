using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float gameDuration = 60f;
    [SerializeField] private float orderInterval = 5f;

    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject timeUpPanel;
    [SerializeField] private Slider timeSlider;

    [SerializeField] private OrderManager orderManager;

    private float timeRemaining;
    private float orderTimer;
    private bool gameRunning = false;

    private void Start()
    {
        timeUpPanel.SetActive(false);
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
            orderManager.GenerateOrder();
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
        orderManager.GenerateOrder();
        UpdateTimerUI();
    }

    private void EndGame()
    {
        gameRunning = false;
        timeUpPanel.SetActive(true);
    }

    private void UpdateTimerUI()
    {
        int seconds = Mathf.CeilToInt(timeRemaining);
        timerText.text = $"Time: {seconds}";

        timeSlider.value = timeRemaining;
    }
}
