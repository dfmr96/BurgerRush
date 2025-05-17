using System.Collections;
using System.Collections.Generic;
using ScriptableObjects.BurgerComplexity;
using TMPro;
using UnityEngine;

public class ScorePopUp : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private float floatDistance = 50f;
    [SerializeField] private float duration = 1f;

    private RectTransform rectTransform;
    private Vector2 startAnchoredPos;
    private float timer;
    private bool isActive;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startAnchoredPos = rectTransform.anchoredPosition;

        // Ocultar el texto al iniciar
        scoreText.enabled = false;
    }

    public void Show(int score, BurgerComplexityData complexity)
    {
        rectTransform.anchoredPosition = startAnchoredPos;
        scoreText.text = $"+{score}";

        var style = complexity.ScorePopupStyle;
        if (style != null)
        {
            scoreText.fontSharedMaterial = style.fontMaterial;
            scoreText.enableVertexGradient = true;
            scoreText.colorGradient = style.Gradient;
        }
        else
        {
            Debug.LogError("[SCOREPOPUP] No material assigned");
        }

        scoreText.enabled = true;

        timer = 0f;
        isActive = true;
        gameObject.SetActive(true);

        var canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }

    private void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;
        float t = timer / duration;

        rectTransform.anchoredPosition = startAnchoredPos + Vector2.up * floatDistance * t;

        var canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
            canvasGroup.alpha = 1f - t;

        if (t >= 1f)
        {
            isActive = false;
            scoreText.enabled = false; // Ocultar texto, pero mantener objeto activo
        }
    }
}
