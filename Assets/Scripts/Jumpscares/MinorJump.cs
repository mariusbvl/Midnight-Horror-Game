using FPC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinorJump : MonoBehaviour
{
    public GameObject sphereTrigger;
    public AudioSource jumpscareSound;

    void OnTriggerEnter(Collider other)
    {
        this.gameObject.GetComponent<BoxCollider>().enabled = false;
        sphereTrigger.SetActive(true);
        StartCoroutine(DeactivateSpehere());
        jumpscareSound.Play();
    }

    IEnumerator DeactivateSpehere()
    {
        yield return new WaitForSeconds(0.5f);
        sphereTrigger.SetActive(false);
    }
}
