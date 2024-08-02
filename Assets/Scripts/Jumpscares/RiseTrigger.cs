using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiseTrigger : MonoBehaviour
{
    public AudioSource RiseSound;

    private void OnTriggerEnter(Collider other)
    {
        this.GetComponent<BoxCollider>().enabled = false;
        RiseSound.Play();
    }
}
