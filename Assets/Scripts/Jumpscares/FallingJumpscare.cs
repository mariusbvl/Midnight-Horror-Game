using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingJumpscare : MonoBehaviour
{
    public GameObject fallingBody;
    public GameObject fallingBodyCollider;
    public AudioSource fallingBodyScream;
    public AudioSource groundHit;

    void OnTriggerEnter(Collider other)
    {
        this.GetComponent<BoxCollider>().enabled = false;
        fallingBodyScream.Play();
        StartCoroutine(PlayImpact());
    }

    IEnumerator PlayImpact()
    {
        yield return new WaitForSeconds(1);
        fallingBody.SetActive(true);
        fallingBody.GetComponent<Animation>().Play("Falling Flat Impact");
        yield return new WaitForSeconds(0.5f);
        groundHit.Play();
        fallingBodyCollider.SetActive(true);
    }
}
