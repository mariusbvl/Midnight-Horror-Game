using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashback_04 : MonoBehaviour
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
    public GameObject bathroomBody;

    public AudioSource enemyScream;
    public AudioSource flashbackBuildup;
    public AudioSource scaredBreathing;
    public AudioSource monsterAttack;
    public AudioSource patientHurt;

    public int delayTime;

    public void PlayFlashback()
    {
        StartCoroutine(ActivateActors());
    }

    IEnumerator ActivateActors()
    {
        yield return new WaitForSeconds(delayTime);
        bathroomBody.SetActive(false);
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
        
        yield return new WaitForSeconds(3.5f);
        flashbackCamera.GetComponent<Animation>().Play("CameraFlashback04Anim");

        scaredBreathing.Play();
        enemy.GetComponent<Animation>().Play("MonsterAttack");
        monsterAttack.Play();
        yield return new WaitForSeconds(0.5f);
        patientHurt.Play();
        patient.GetComponent<Animation>().Play("Fall Flat");
        enemyScream.Play();

        yield return new WaitForSeconds(1);
        flashbackFadeOut.SetActive(true);

        yield return new WaitForSeconds(3);
        scaredBreathing.Stop();
        flashbackFade.SetActive(false);
        flashbackCamera.SetActive(false);
        patient.SetActive(false);
        enemy.SetActive(false);
        oldVolume.SetActive(true);
        flashbackVolume.SetActive(false);
        playerHUD.SetActive(true);
        playerMesh.SetActive(true);
        camHolder.SetActive(true);
        flashbackFadeOut.SetActive(false);
        bathroomBody.SetActive(true);
    }
}
