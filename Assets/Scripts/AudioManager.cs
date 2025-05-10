using System.Collections.Generic;
using DefaultNamespace.Enums;
using ScriptableObjects;
using ScriptableObjects.BurgerComplexityData;
using UnityEngine;
using UnityEngine.Audio;

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
    
    [SerializeField] private int pooledSourcesCount = 2;
    private List<AudioSource> pooledSources = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSFXPool();
        }
        else
        {
            Destroy(gameObject);
        }

        LoadSavedVolumes();
    }
    
    private void InitializeSFXPool()
    {
        for (int i = 0; i < pooledSourcesCount; i++)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = sfxSource.outputAudioMixerGroup;
            source.playOnAwake = false;
            pooledSources.Add(source);
        }
    }
    
    private AudioSource GetAvailableSource()
    {
        foreach (var source in pooledSources)
        {
            if (!source.isPlaying)
                return source;
        }

        // Si todos están ocupados, opcionalmente podés clonar uno (o retornar null)
        var newSource = gameObject.AddComponent<AudioSource>();
        newSource.outputAudioMixerGroup = sfxSource.outputAudioMixerGroup;
        newSource.playOnAwake = false;
        pooledSources.Add(newSource);
        return newSource;
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
    public void PlayOrderDeliveredSFX(int combo)
    {
        if (sfxLibrary.clips.TryGetValue(SFXType.OrderDelivered, out var clip) && clip != null)
        {
            float pitch = Mathf.Lerp(0.8f, 1.5f, Mathf.InverseLerp(3, 21, combo));
            Debug.Log($"🎧 Playing OrderDelivered with pitch: {pitch} | Combo: {combo} | Clip: {clip.name}");

            AudioSource source = GetAvailableSource();
            if (source != null)
            {
                source.pitch = pitch;
                source.PlayOneShot(clip);
            }
        }
    }
    
    public void PlayIngredientPlacedSFX()
    {
        if (sfxLibrary.clips.TryGetValue(SFXType.IngredientPlaced, out var clip) && clip != null)
        {
            AudioSource source = GetAvailableSource();
            if (source != null)
            {
                //float randomPitch = Random.Range(.9f, 1.1f);
                //source.pitch = randomPitch;
                source.PlayOneShot(clip);
            }
        }
    }
    
    public void PlayNewOrderSFX(BurgerComplexityData complexity)
    {
        if (sfxLibrary.clips.TryGetValue(SFXType.NewOrder, out var clip) && clip != null && complexity != null)
        {
            AudioSource source = GetAvailableSource();
            if (source != null)
            {
                float pitch = complexity.OrderCreatedPitch;
                source.pitch = pitch;
                source.PlayOneShot(clip);
            }
        }
    }
}