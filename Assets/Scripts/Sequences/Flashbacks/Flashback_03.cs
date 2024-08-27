using System.Collections;
using System.Collections.Generic;
using FPC;
using UnityEngine;

public class Flashback_03 : MonoBehaviour
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

    public int delayTime;


    public void PlayFlashback()
    {
        StartCoroutine(ActivateActors());
    }

    IEnumerator ActivateActors()
    {
        yield return new WaitForSeconds(delayTime);
        InteractController.Instance.characterController.enabled = false;
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
        yield return new WaitForSeconds(2.5f);
        flashbackCamera.GetComponent<Animation>().Play("CameraFlashback03Anim");

        scaredBreathing.Play();
        patient.GetComponent<Animation>().Play("Crouch Look Around Corner");
        yield return new WaitForSeconds(3.5f);
        enemy.GetComponent<Animation>().Play("ScreamAnim");
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
        InteractController.Instance.characterController.enabled = true;
    }
}
