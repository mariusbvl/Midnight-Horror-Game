using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderJumpscare : MonoBehaviour
{
    public GameObject spider;
    public GameObject playerHands;
    public GameObject playerCursor;

    public AudioSource spiderSound;
    public AudioSource jumpscareSound;

    void OnTriggerEnter(Collider other)
    {
        this.GetComponent<BoxCollider>().enabled = false;
        StartCoroutine(PlayImpact());
    }

    IEnumerator PlayImpact()
    {
        playerHands.SetActive(false);
        playerCursor.SetActive(false);

        jumpscareSound.Play();
        spider.SetActive(true);
        spiderSound.Play();
        yield return new WaitForSeconds(0.25f);
        spider.GetComponent<Animation>().Play("SpiderWalking");

        yield return new WaitForSeconds(3);
        spiderSound.Stop();

        spider.SetActive(false);
        playerHands.SetActive(true);
        playerCursor.SetActive(true);
    }
}
