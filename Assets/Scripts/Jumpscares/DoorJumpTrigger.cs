using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorJumpTrigger : MonoBehaviour
{
    public AudioSource DoorBang;
    public AudioSource DoorJumpSFX;
    public AudioSource MonsterScream;
    public GameObject Enemy;
    public GameObject Door;

    private void OnTriggerEnter(Collider other)
    {
        this.GetComponent<BoxCollider>().enabled = false;
        Door.GetComponent<Animation>().Play("JumpScareDoorAnim");
        DoorBang.Play();
        Enemy.SetActive(true);
        StartCoroutine(PlayJumpSFX());
    }

    IEnumerator PlayJumpSFX()
    {
        yield return new WaitForSeconds(0.3f);
        DoorJumpSFX.Play();
        yield return new WaitForSeconds(0.2f);
        MonsterScream.Play();
        yield return new WaitForSeconds(3);
        Enemy.SetActive(false);
    }
}
