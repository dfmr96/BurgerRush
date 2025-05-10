using System;
using Enums;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private SFXType mainMenuThemeSFX;
    private void Start()
    {
        AudioManager.Instance.PlayBackgroundMusic(mainMenuThemeSFX);
    }
}