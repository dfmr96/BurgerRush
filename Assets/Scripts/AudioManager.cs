using DefaultNamespace.Enums;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Audio;

namespace DefaultNamespace
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [Header("Mixer References")]
        [SerializeField] private AudioMixer audioMixer;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip previewMusicClip;
        [SerializeField] private AudioClip previewSfxClip;
        
        [Header("SFX Library")]
        [SerializeField] private SFXLibrary sfxLibrary;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadSavedVolumes(); // opcional
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetMasterVolume(float value)
        {
            audioMixer.SetFloat("MasterVolume", LinearToDecibel(value));
            PlayerPrefs.SetFloat("MasterVolume", value);
        }

        public void SetMusicVolume(float value)
        {
            audioMixer.SetFloat("MusicVolume", LinearToDecibel(value));
            PlayerPrefs.SetFloat("MusicVolume", value);
        }

        public void SetSFXVolume(float value)
        {
            audioMixer.SetFloat("SFXVolume", LinearToDecibel(value));
            PlayerPrefs.SetFloat("SFXVolume", value);
        }

        public void PlayBackgroundMusic()
        {
            if (previewMusicClip)
            {
                musicSource.clip = previewMusicClip;
                musicSource.loop = false;
                musicSource.Play();
            }
        }

        public void PlayButtonClickSound()
        {
            if (previewSfxClip)
                sfxSource.PlayOneShot(previewSfxClip);
        }

        private void LoadSavedVolumes()
        {
            SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 1f));
            SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 0.7f));
            SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1f));
        }

        private float LinearToDecibel(float value)
        {
            return Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
        }
        
        public float GetSavedVolume(string key, float defaultValue)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }
        
        public void PlaySFX(SFXType type)
        {
            if (sfxLibrary.clips.TryGetValue(type, out var clip) && clip != null)
            {
                sfxSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning($"🎵 SFX clip for {type} not found!");
            }
        }
    }
}