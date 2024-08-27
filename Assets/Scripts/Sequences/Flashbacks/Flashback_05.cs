using System.Collections;
using System.Collections.Generic;
using FPC;
using UnityEngine;

public class Flashback_05 : MonoBehaviour
{
    public GameObject flashbackFade;
    public GameObject flashbackFadeOut;
    public GameObject flashbackCamera_01;
    public GameObject flashbackCamera_02;
    public GameObject flashbackVolume;
    public GameObject oldVolume;
    public GameObject playerHUD;
    public GameObject patient;
    public GameObject playerMesh;
    public GameObject camHolder;
    public GameObject hangedBody;
    public GameObject rope;

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
        flashbackCamera_01.SetActive(true);
        oldVolume.SetActive(false);
        flashbackVolume.SetActive(true);
        playerHUD.SetActive(false);
        playerMesh.SetActive(false);
        camHolder.SetActive(false);
        hangedBody.SetActive(false);
        patient.SetActive(true);
        rope.SetActive(true);

        yield return new WaitForSeconds(3.5f);
        flashbackCamera_01.GetComponent<Animation>().Play("CameraFlashback05Anim");

        scaredBreathing.Play();
        yield return new WaitForSeconds(0.5f);
        patient.GetComponent<Animation>().Play("Nervously Look Around");
        yield return new WaitForSeconds(1);

        yield return new WaitForSeconds(5);
        
        flashbackCamera_01.SetActive(false);
        patient.SetActive(false);
        flashbackCamera_02.SetActive(true);
        yield return new WaitForSeconds(1);
        enemyScream.Play();
        scaredBreathing.Stop();
        yield return new WaitForSeconds(2);

        flashbackFadeOut.SetActive(true);

        yield return new WaitForSeconds(3);
        scaredBreathing.Stop();
        flashbackFade.SetActive(false);
        flashbackCamera_02.SetActive(false);
        oldVolume.SetActive(true);
        flashbackVolume.SetActive(false);
        playerHUD.SetActive(true);
        playerMesh.SetActive(true);
        camHolder.SetActive(true);
        flashbackFadeOut.SetActive(false);
        rope.SetActive(false);
        hangedBody.SetActive(true);
        InteractController.Instance.characterController.enabled = true;
    }
}
