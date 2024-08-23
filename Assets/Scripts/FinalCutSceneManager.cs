using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalCutSceneManager : MonoBehaviour
{
    [SerializeField] private LoadingManager loadingManager;

    private void Start()
    {
        SoundMixerManager.Instance.SetMasterVolume(SaveManager.Instance.masterVolumeValue);
        SoundMixerManager.Instance.SetMusicVolume(SaveManager.Instance.musicVolumeValue);
        SoundMixerManager.Instance.SetSoundFXVolume(SaveManager.Instance.sfxVolumeValue);
        StartCoroutine(WaitForTimeLineToEnd());
    }

    private IEnumerator WaitForTimeLineToEnd()
    {
        yield return new WaitForSeconds(39f);
        loadingManager.LoadScene(5);
    }
    
}
