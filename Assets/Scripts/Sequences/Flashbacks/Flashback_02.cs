using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashback_02 : MonoBehaviour
{
    public GameObject flashbackFade;
    public GameObject flashbackFadeOut;
    public GameObject flashbackCamera;
    public GameObject flashbackVolume;
    public GameObject oldVolume;
    public GameObject playerHUD;
    public GameObject patient;
    public GameObject enemy;
    public GameObject playerMesh;
    public GameObject camHolder;

    public AudioSource enemyScream;
    public AudioSource flashbackBuildup;
    public AudioSource scaredBreathing;
    public AudioSource monsterRunning;
    public AudioSource monsterAttack;

    public int delayTime;


    public void PlayFlashback()
    {
        StartCoroutine(ActivateActors());
    }

    IEnumerator ActivateActors()
    {
        yield return new WaitForSeconds(delayTime);
        flashbackBuildup.Play();
        flashbackFade.SetActive(true);
        flashbackCamera.SetActive(true);
        oldVolume.SetActive(false);
        flashbackVolume.SetActive(true);
        playerHUD.SetActive(false);
        playerMesh.SetActive(false);
        camHolder.SetActive(false);
        patient.SetActive(true);
        enemy.SetActive(true);
        yield return new WaitForSeconds(2);

        scaredBreathing.Play();
        monsterRunning.Play();
        yield return new WaitForSeconds(0.5f);
        enemy.GetComponent<Animation>().Play("StandardRun");
        patient.GetComponent<Animation>().Play("Injured Run");
        yield return new WaitForSeconds(4.5f);
        monsterAttack.Play();
        monsterRunning.Stop();
        yield return new WaitForSeconds(0.5f);
        enemyScream.Play();

        enemy.SetActive(false);
        yield return new WaitForSeconds(1);
        flashbackFadeOut.SetActive(true);
        patient.SetActive(false);

        yield return new WaitForSeconds(3);
        scaredBreathing.Stop();
        flashbackFade.SetActive(false);
        flashbackCamera.SetActive(false);
        oldVolume.SetActive(true);
        flashbackVolume.SetActive(false);
        playerHUD.SetActive(true);
        playerMesh.SetActive(true);
        camHolder.SetActive(true);
        flashbackFadeOut.SetActive(false);
    }
}
