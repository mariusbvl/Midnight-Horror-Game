using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayJumpscare : MonoBehaviour
{
    public GameObject hallwayLight_01;
    public GameObject hallwayLight_02;

    public AudioSource distractionSound;
    public AudioSource lightbulbBreakSound_01;
    public AudioSource lightbulbBreakSound_02;

    private void OnTriggerEnter(Collider other)
    {
        this.GetComponent<BoxCollider>().enabled = false;
        StartCoroutine(PlayJumpscare());
    }

    IEnumerator PlayJumpscare()
    {
        distractionSound.Play();
        yield return new WaitForSeconds(2);
        lightbulbBreakSound_01.Play();
        hallwayLight_01.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        lightbulbBreakSound_02.Play();
        hallwayLight_02.SetActive(false);
    }
}
