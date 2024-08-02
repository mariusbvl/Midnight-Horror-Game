using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PrisonJumpTrigger : MonoBehaviour
{
    public GameObject PrisonInmate;
    public AudioSource InmateScream;
    public AudioSource JumpSFX;

    void OnTriggerEnter(Collider other)
    {
        this.GetComponent<BoxCollider>().enabled = false;
        PrisonInmate.SetActive(true);
        InmateScream.Play();
        StartCoroutine(PlayImpact());
    }

    IEnumerator PlayImpact()
    {
        yield return new WaitForSeconds(1);
        JumpSFX.Play();
    }
}
