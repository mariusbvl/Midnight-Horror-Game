using System.Collections;
using UnityEngine;

public class Flashback_01 : MonoBehaviour
{
    public GameObject[] elementsToActivate;
    public GameObject flashbackFade;
    public GameObject flashbackFadeOut;
    public GameObject camera;
    public GameObject flashbackVolume;
    public GameObject oldVolume;
    public GameObject playerHUD;
    public GameObject door;
    public GameObject patient;
    public GameObject enemy;
    public GameObject playerMesh;
    public GameObject camHolder;

    public AudioSource doorBang;
    public AudioSource enemyScream;
    public AudioSource flashbackBuildup;
    public AudioSource scaredBreathing;


    public void PlayFlashback()
    {
        StartCoroutine(ActivateActors());
    }

    IEnumerator ActivateActors()
    {
        flashbackBuildup.Play();
        flashbackFade.SetActive(true);
        camera.SetActive(true);
        oldVolume.SetActive(false);
        flashbackVolume.SetActive(true);
        playerHUD.SetActive(false);
        playerMesh.SetActive(false);
        camHolder.SetActive(false);
        patient.SetActive(true);
        enemy.SetActive(true);
        yield return new WaitForSeconds(3);

        scaredBreathing.Play();
        doorBang.Play();
        enemy.GetComponent<Animation>().Play("mixamo.com");
        door.GetComponent<Animation>().Play("BashDoorFlashback01Anim");
        patient.GetComponent<Animation>().Play("Walk Backwards 2");
        yield return new WaitForSeconds(2.5f);
        enemy.GetComponent<Animation>().Play("ScreamAnim");
        yield return new WaitForSeconds(0.5f);
        enemyScream.Play();

        yield return new WaitForSeconds(1.4f);
        flashbackFadeOut.SetActive(true);
        
        yield return new WaitForSeconds(3);
        scaredBreathing.Stop();
        flashbackFade.SetActive(false);
        camera.SetActive(false);
        oldVolume.SetActive(true);
        flashbackVolume.SetActive(false);
        playerHUD.SetActive(true);
        patient.SetActive(false);
        enemy.SetActive(false);
        playerMesh.SetActive(true);
        camHolder.SetActive(true);
        flashbackFadeOut.SetActive(false);
    }
}
