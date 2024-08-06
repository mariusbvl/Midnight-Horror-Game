using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverSequence : MonoBehaviour
{
    public GameObject Blood;
    public AudioSource enemySound;
    public AudioSource scareSound;
    public GameObject fadeOutScreen;

    private void Start()
    {
        StartCoroutine(SequenceBegin());
    }

    IEnumerator SequenceBegin()
    {
        yield return new WaitForSeconds(1);
        Blood.SetActive(true);
        enemySound.Play();
        yield return new WaitForSeconds(2.5f);
        scareSound.Play();
        yield return new WaitForSeconds(2);
        fadeOutScreen.SetActive(true);
    }
}
