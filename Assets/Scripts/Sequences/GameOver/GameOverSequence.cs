using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class GameOverSequence : MonoBehaviour
{
    public GameObject Blood;
    public AudioSource enemySound;
    public AudioSource scareSound;
    public GameObject fadeOutScreen;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;

    private void Start()
    {
        SoundMixerManager.Instance.SetMusicVolume(1f);
        SoundMixerManager.Instance.SetSoundFXVolume(1f);
        StartCoroutine(SequenceBegin());
    }

    IEnumerator SequenceBegin()
    {
        yield return new WaitForSeconds(1);
        Blood.SetActive(true);
        enemySound.Play();
        yield return new WaitForSeconds(3.5f);
        scareSound.Play();
        yield return new WaitForSeconds(1.5f);
        fadeOutScreen.SetActive(true);
        yield return new WaitForSeconds(2f);
        gameOverPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(restartButton.gameObject);
    }
}
