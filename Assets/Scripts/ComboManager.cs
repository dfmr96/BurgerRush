using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private int minComboStreak = 3;
    [field:SerializeField] private int currentStreak = 0;
    [SerializeField] private Gradient comboGradient;
    private bool comboActive = false;

    public int CurrentStreak => currentStreak;

    private void Start()
    {
        comboText.SetText(string.Empty);
    }

    public void RegisterDelivery()
    {
        currentStreak = CurrentStreak + 1;
        PlayerStatsManager.UpdateMaxStreak(CurrentStreak);


        if (CurrentStreak >= minComboStreak)
        {
            comboActive = true;
            UpdateComboUI();
        }
    }

    public float GetMultiplier()
    {
        if (CurrentStreak < 3) return 1f;

        int cappedStreak = Mathf.Min(CurrentStreak, 21);
        float multiplier = 1f + ((cappedStreak - 2) * 0.1f); // 3 = x1.1, ..., 21+ = x2.0
        return Mathf.Min(multiplier, 2.0f);
    }

    public void ResetCombo()
    {
        if (!comboActive) return; // Protección

        currentStreak = 0;
        comboActive = false;
        comboText.gameObject.SetActive(false);
    }

    private void UpdateComboUI()
    {
        comboText.gameObject.SetActive(true);
        float t = Mathf.InverseLerp(3, 21, CurrentStreak);
        comboText.color = comboGradient.Evaluate(t);
        comboText.text = $"COMBO x{GetMultiplier():0.0}!";
    }
}
