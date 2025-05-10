using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.Enums;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlaySoundOnClick : MonoBehaviour
{
    [SerializeField] private SFXType soundToPlay;

    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();

        if (button == null) return;
        button.onClick.AddListener(PlaySound);
    }

    private void PlaySound()
    {
        AudioManager.Instance.PlaySFX(soundToPlay);
    }
}
