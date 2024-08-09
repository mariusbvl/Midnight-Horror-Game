using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterShootingScene : MonoBehaviour
{
    public GameObject fadeIn;
    public GameObject fadeOut;
    public GameObject shootingCutsceneCamera;
    public GameObject playerHUD;
    public GameObject enemy;
    public GameObject playerMesh;
    public GameObject camHolder;
    public GameObject fog;

    public AudioSource monsterScream;

    private void OnTriggerEnter(Collider other)
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
        StartCoroutine(ActivateActors());
    }

    IEnumerator ActivateActors()
    {
        playerHUD.SetActive(false);
        playerMesh.SetActive(false);
        camHolder.SetActive(false);
        fog.SetActive(false);

        shootingCutsceneCamera.SetActive(true);
        fadeOut.SetActive(true);
        enemy.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        enemy.GetComponent<Animation>().Play("ScreamAnim");
        monsterScream.Play();

        yield return new WaitForSeconds(12);
        playerHUD.SetActive(true);
        playerMesh.SetActive(true);
        camHolder.SetActive(true);
        shootingCutsceneCamera.SetActive(false);
        fog.SetActive(true);
    }
}
