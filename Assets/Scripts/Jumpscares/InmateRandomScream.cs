using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InmateRandomScream : MonoBehaviour
{
    public int ScreamMode;
    public AudioSource ConsistentSFX_01;
    public AudioSource ConsistentSFX_02;
    public AudioSource ConsistentSFX_03;
    public AudioSource ConsistentSFX_04;
    public AudioSource ConsistentSFX_05;
    public AudioSource ConsistentSFX_06;
    public AudioSource ConsistentSFX_07;

    void Update()
    {
        if (ScreamMode == 0)
        {
            StartCoroutine(PlayScream());
        }
    }

    IEnumerator PlayScream()
    {
        ScreamMode = Random.Range(1, 8);
        if (ScreamMode == 1)
        {
            ConsistentSFX_01.Play();
            yield return new WaitForSeconds(5);
        }
        if (ScreamMode == 2)
        {
            ConsistentSFX_02.Play();
            yield return new WaitForSeconds(5);
        }
        if (ScreamMode == 3)
        {
            ConsistentSFX_03.Play();
            yield return new WaitForSeconds(5);
        }
        if (ScreamMode == 4)
        {
            ConsistentSFX_04.Play();
            yield return new WaitForSeconds(5);
        }
        if (ScreamMode == 5)
        {
            ConsistentSFX_05.Play();
            yield return new WaitForSeconds(5);
        }
        if (ScreamMode == 6)
        {
            ConsistentSFX_06.Play();
            yield return new WaitForSeconds(5);
        }
        if (ScreamMode == 7)
        {
            ConsistentSFX_07.Play();
            yield return new WaitForSeconds(5);
        }
        ScreamMode = 0;
    }
}
