using System;
using System.Collections.Generic;
using Databases;
using Enums;
using ScriptableObjects;
using ScriptableObjects.BurgerComplexity;
using Services.Utils;
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
    
    private List<AudioSource> pooledSpatialSources = new();
    [SerializeField] private int pooledSpatialCount = 4;

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
    }

    private void Start()
    {
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

        for (int i = 0; i < pooledSpatialCount; i++)
        {
            var obj = new GameObject($"SpatialSource_{i}");
            obj.transform.parent = transform; // organizarlos
            var spatialSource = obj.AddComponent<AudioSource>();
            spatialSource.outputAudioMixerGroup = sfxSource.outputAudioMixerGroup;
            spatialSource.playOnAwake = false;
            spatialSource.spatialBlend = 1f;
            spatialSource.minDistance = 1f;
            spatialSource.maxDistance = 10f;
            spatialSource.rolloffMode = AudioRolloffMode.Linear;

            pooledSpatialSources.Add(spatialSource);
        }
    }
    
    private AudioSource GetAvailableSource(Vector3? position = null)
    {
        if (position == null)
        {
            foreach (var source in pooledSources)
            {
                if (!source.isPlaying)
                    return source;
            }

            var newSource = gameObject.AddComponent<AudioSource>();
            newSource.outputAudioMixerGroup = sfxSource.outputAudioMixerGroup;
            newSource.playOnAwake = false;
            pooledSources.Add(newSource);
            return newSource;
        }
        else
        {
            foreach (var source in pooledSpatialSources)
            {
                if (!source.isPlaying)
                {
                    source.transform.position = position.Value;
                    return source;
                }
            }

            // Si están todos ocupados, podés decidir si querés crear más o no:
            var sfxObject = new GameObject("Dynamic_SpatialSource");
            sfxObject.transform.position = position.Value;

            var audioSource = sfxObject.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = sfxSource.outputAudioMixerGroup;
            audioSource.spatialBlend = 1f;
            audioSource.minDistance = 1f;
            audioSource.maxDistance = 10f;
            audioSource.playOnAwake = false;
            audioSource.rolloffMode = AudioRolloffMode.Linear;

            pooledSpatialSources.Add(audioSource); // opcional, para ampliar el pool
            return audioSource;
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


    private void LoadSavedVolumes()
    {
        SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 1));
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 1));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1));
        Debug.Log($"MASTER VOLUME: {PlayerPrefs.GetFloat("MasterVolume", 0)}");
        Debug.Log($"MUSIC VOLUME:  {PlayerPrefs.GetFloat("MusicVolume", 0)}");
        Debug.Log($"SFX VOLUME: {PlayerPrefs.GetFloat("SFXVolume", 0)}");
    }

    private float LinearToDecibel(float value)
    {
        return Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
    }
        
    public float GetSavedVolume(string key, float defaultValue)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }
    
    public void SetMusicPitch(float pitch)
    {
        musicSource.pitch = pitch;
    }
        
    public void PlayBackgroundMusicSample()
    {
        if (previewMusicClip)
        {
            musicSource.clip = previewMusicClip;
            musicSource.loop = false;
            musicSource.Play();
        }
    }
    
    public void PlayBackgroundMusic(SFXType musicType)
    {
        if (sfxLibrary.clips.TryGetValue(musicType, out var clip) && clip != null)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"🎵 Music clip for {musicType} not found!");
        }

        bool shouldLoop = musicType != SFXType.GameOverTheme;
        musicSource.loop = shouldLoop;
    }

    public void PlayButtonClickSound()
    {
        if (previewSfxClip)
            sfxSource.PlayOneShot(previewSfxClip);
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
    
    public void PlayIngredientPlacedSFX(RectTransform sourceUI = null)
    {
        if (sfxLibrary.clips.TryGetValue(SFXType.IngredientPlaced, out var clip) && clip != null)
        {
            AudioSource source;

            if (sourceUI != null)
            {
                Vector3 worldPos = WorldPositionHelper.GetWorldPositionFromUI(sourceUI);
                source = GetAvailableSource(worldPos);
            }
            else
            {
                source = GetAvailableSource(); // No 3D
            }

            if (source != null)
            {
                source.PlayOneShot(clip);
            }
        }
    }
    
    public void PlayNewOrderSFX(BurgerComplexityData complexity, RectTransform sourceUI = null)
    {
        if (sfxLibrary.clips.TryGetValue(SFXType.NewOrder, out var clip) && clip != null && complexity != null)
        {
            Vector3? pos = null;

            if (sourceUI != null)
                pos = WorldPositionHelper.GetWorldPositionFromUI(sourceUI);

            AudioSource source = GetAvailableSource(pos);
            if (source != null)
            {
                float pitch = complexity.OrderCreatedPitch;
                source.pitch = pitch;
                source.PlayOneShot(clip);
            }
        }
    }
}