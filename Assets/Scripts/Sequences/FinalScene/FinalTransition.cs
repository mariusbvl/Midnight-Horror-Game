using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalTransition : MonoBehaviour
{
    public GameObject fadeScreen;
    public AudioSource wolfSound;

    void Start()
    {
        StartCoroutine(ActivateActors());
    }

    IEnumerator ActivateActors()
    {
        yield return new WaitForSeconds(33);
        wolfSound.Play();
        yield return new WaitForSeconds(2);
        fadeScreen.SetActive(true);
    }
}
