using FPC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterShootingScene : MonoBehaviour
{
    public GameObject fadeIn;
    public GameObject fadeOut;
    public GameObject player;
    public GameObject playerHUD;
    public GameObject enemy;
    public GameObject playerMesh;
    public GameObject camHolder;
    public GameObject fog;
    public GameObject monsterBlood;

    public GameObject camera_01;
    public GameObject camera_02;
    public GameObject camera_03;
    public GameObject camera_04;
    public GameObject camera_05;

    public GameObject gunshot_01;
    public GameObject gunshot_02;
    public GameObject gunshot_03;
    public GameObject gunshot_04;

    public AudioSource monsterScream;
    public AudioSource monsterDieScream;
    public AudioSource warAmbience;
    public AudioSource shot_01;
    public AudioSource shot_02;
    public AudioSource shot_03;
    public AudioSource music;
    public AudioSource wolfSound;

    private void OnTriggerEnter(Collider other)
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
        StartCoroutine(ActivateActors());
    }

    IEnumerator ActivateActors()
    {
        player.GetComponent<FirstPersonController>().enabled = false;
        playerHUD.SetActive(false);
        playerMesh.SetActive(false);
        camHolder.SetActive(false);
        fog.SetActive(false);

        camera_01.SetActive(true);
        fadeOut.SetActive(true);
        enemy.SetActive(true);
        enemy.GetComponent<Animation>().Play("ScreamAnim");
        yield return new WaitForSeconds(0.5f);
        monsterScream.Play();

        yield return new WaitForSeconds(2.5f);
        camera_02.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        gunshot_01.SetActive(true);
        shot_01.Play();
        yield return new WaitForSeconds(0.1f);
        gunshot_01.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        gunshot_01.SetActive(true);
        shot_01.Play();
        yield return new WaitForSeconds(0.1f);
        gunshot_01.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        gunshot_01.SetActive(true);
        shot_01.Play();
        yield return new WaitForSeconds(0.1f);
        gunshot_01.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        gunshot_01.SetActive(true);
        shot_01.Play();
        yield return new WaitForSeconds(0.3f);
        gunshot_01.SetActive(false);

        camera_03.SetActive(true);
        warAmbience.Play();

        yield return new WaitForSeconds(0.5f);
        gunshot_02.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gunshot_02.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        gunshot_03.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gunshot_03.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        gunshot_04.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gunshot_04.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        gunshot_02.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gunshot_02.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        gunshot_03.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gunshot_03.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        gunshot_04.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gunshot_04.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        gunshot_02.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gunshot_02.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        gunshot_03.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gunshot_03.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        gunshot_04.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gunshot_04.SetActive(false);


        camera_04.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        gunshot_01.SetActive(true);
        shot_01.Play();
        yield return new WaitForSeconds(0.1f);
        gunshot_01.SetActive(false);

        camera_05.SetActive(true);
        warAmbience.Stop();
        monsterDieScream.Play();
        enemy.GetComponent<Animation>().Play("DieAnim");
        monsterBlood.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        monsterBlood.SetActive(false);
        camera_05.GetComponent<Animation>().Play("DeathSceneAnim");
        yield return new WaitForSeconds(10);
        wolfSound.Play();
        yield return new WaitForSeconds(5);
        fadeIn.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        music.Stop();

        warAmbience.Stop();
        fadeIn.SetActive(false);
        playerHUD.SetActive(true);
        playerMesh.SetActive(true);
        camHolder.SetActive(true);
        camera_01.SetActive(false);
        camera_02.SetActive(false);
        camera_03.SetActive(false);
        camera_04.SetActive(false);
        camera_05.SetActive(false);
        fog.SetActive(true);
        player.GetComponent<FirstPersonController>().enabled = true;
    }
}
