using Enums;
using UnityEngine;

public class MainMenuAudioController : MonoBehaviour
{
    [SerializeField] private SFXType mainMenuTheme;

    private void Start()
    {
        AudioManager.Instance.PlayBackgroundMusic(mainMenuTheme);
        AudioManager.Instance.SetMusicPitch(1f);
    }
}