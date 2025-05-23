﻿using Services.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameplayUIManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionPanel;
    public void OnPausePressed()
    {
        Time.timeScale = 0f;
        EventSystem.current.SetSelectedGameObject(null);
        Debug.Log("Pause Pressed");
        pausePanel.SetActive(true);
    }

    public void OnResumePressed()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnRestartPressed()
    {
        Time.timeScale = 1f;
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SendCustomEndEvent();
        }
        
        RestartGameHelper.RestartSceneWithInterstitial();
    }

    public void OnMainMenuPressed()
    {
        Time.timeScale = 1f;

        AdOverlayHandler.ShowInterstitialWithBannerRestore(() =>
        {
            AdsManager.Instance.IncrementPlayCount();
            SceneManager.LoadScene("MainMenu");
        });
    }
        
    public void OnOptionPressed()
    {
        Debug.Log("Option Pressed");
        optionPanel.SetActive(true);
        Canvas.ForceUpdateCanvases();
    }
}