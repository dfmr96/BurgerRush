using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    [SerializeField] private float comboWindow = 5f;
    [SerializeField] private TMP_Text comboText;

    private float timer;
    private int currentStreak = 0;
    private bool comboActive = false;

    private void Update()
    {
        if (!comboActive) return;

        timer += Time.deltaTime;
        if (timer >= comboWindow)
        {
            ResetCombo();
        }
    }

    public void RegisterDelivery()
    {
        timer = 0f;
        currentStreak++;

        if (currentStreak >= 3)
        {
            comboActive = true;
            UpdateComboUI();
        }
    }

    public float GetMultiplier()
    {
        if (currentStreak < 3) return 1f;

        int cappedStreak = Mathf.Min(currentStreak, 21);
        float multiplier = 1f + ((cappedStreak - 2) * 0.1f); // 3 = x1.1, ..., 21+ = x2.0
        return Mathf.Min(multiplier, 2.0f);
    }

    public void ResetCombo()
    {
        currentStreak = 0;
        comboActive = false;
        comboText.gameObject.SetActive(false);
    }

    private void UpdateComboUI()
    {
        comboText.gameObject.SetActive(true);
        comboText.text = $"COMBO x{GetMultiplier():0.0}!";
    }
}
