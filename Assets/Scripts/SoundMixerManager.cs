using System;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    public static SoundMixerManager Instance { get; private set; }
    [SerializeField] private AudioMixer mainMixer;
    
    [Range(0.0001f, 1f)] public float masterVolume;
    [Range(0.0001f, 1f)] public float soundFXVolume;
    [Range(0.0001f, 1f)] public float musicVolume;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void SetMasterVolume(float level)
    {
        mainMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20f);
    }
    
    public void SetSoundFXVolume(float level)
    {
        mainMixer.SetFloat("soundFXVolume", Mathf.Log10(level) * 20f);
    }
    
    public void SetMusicVolume(float level)
    {
        mainMixer.SetFloat("musicVolume", Mathf.Log10(level) * 20f);
    }
}
