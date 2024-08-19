using FPC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class MonsterShootingScene : MonoBehaviour
{
    [SerializeField] private LoadingManager loadingManager;
    public GameObject fadeIn;
    public GameObject fadeOut;
    public GameObject player;
    public GameObject playerHUD;
    public GameObject enemy;
    public GameObject playerMesh;
    public GameObject camHolder;
    public GameObject monsterBlood;
    public GameObject newspaper;
    public GameObject newspaperBase;

    public GameObject cursor;
    public GameObject batteryIcon;

    public GameObject camera_01;
    public GameObject camera_02;
    public GameObject camera_03;
    public GameObject camera_04;
    public GameObject camera_05;

    public GameObject gunshot_01;
    public GameObject gunshot_02;
    public GameObject gunshot_03;
    public GameObject gunshot_04;

    public GameObject rig_1;
    public GameObject rig_2;
    public GameObject rig_3;
    public GameObject rig_4;

    public GameObject swat_01;
    public GameObject swat_02;
    public GameObject swat_03;
    public GameObject swat_04;

    public GameObject gun_01;
    public GameObject gun_02;
    public GameObject gun_03;
    public GameObject gun_04;
    public GameObject gun_01_anim;
    public GameObject gun_02_anim;
    public GameObject gun_03_anim;
    public GameObject gun_04_anim;

    public AudioSource monsterScream;
    public AudioSource monsterDieScream;
    public AudioSource warAmbience;
    public AudioSource shot_01;
    public AudioSource shot_02;
    public AudioSource shot_03;
    public AudioSource music;
    public AudioSource wolfSound;
    public AudioSource newspaperSound;

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

        rig_1.GetComponent<Rig>().weight = 0;
        rig_2.GetComponent<Rig>().weight = 0;
        rig_3.GetComponent<Rig>().weight = 0;
        rig_4.GetComponent<Rig>().weight = 0;

        camera_01.SetActive(true);
        fadeOut.SetActive(true);
        enemy.SetActive(true);
        enemy.GetComponent<Animation>().Play("ScreamAnim");
        yield return new WaitForSeconds(0.5f);
        monsterScream.Play();
        
        gun_01.SetActive(false);
        gun_01_anim.SetActive(true);
        yield return new WaitForSeconds(2);
        camera_02.SetActive(true);

        swat_01.GetComponent<Animation>().Play("Shooting");
        gunshot_01.SetActive(true);
        shot_01.Play();
        yield return new WaitForSeconds(0.1f);
        gunshot_01.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        yield return new WaitForSeconds(1);
        swat_01.GetComponent<Animation>().Play("Shooting");
        gunshot_01.SetActive(true);
        shot_01.Play();
        yield return new WaitForSeconds(0.3f);
        gunshot_01.SetActive(false);
        yield return new WaitForSeconds(1);

        camera_03.SetActive(true);
        warAmbience.Play();

        // Start Shoot Scene

        swat_02.GetComponent<Animation>().Play("Shooting");
        yield return new WaitForSeconds(0.5f);
        gunshot_02.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gunshot_02.SetActive(false);


        swat_03.GetComponent<Animation>().Play("Shooting");
        yield return new WaitForSeconds(0.5f);
        gunshot_03.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gunshot_03.SetActive(false);


        swat_04.GetComponent<Animation>().Play("Shooting");
        yield return new WaitForSeconds(0.5f);
        gunshot_04.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gunshot_04.SetActive(false);


        swat_02.GetComponent<Animation>().Play("Shooting");
        yield return new WaitForSeconds(0.5f);
        gunshot_02.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gunshot_02.SetActive(false);


        swat_03.GetComponent<Animation>().Play("Shooting");
        yield return new WaitForSeconds(0.5f);
        gunshot_03.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gunshot_03.SetActive(false);


        swat_04.GetComponent<Animation>().Play("Shooting");
        yield return new WaitForSeconds(0.5f);
        gunshot_04.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gunshot_04.SetActive(false);


        swat_02.GetComponent<Animation>().Play("Shooting");
        yield return new WaitForSeconds(0.5f);
        gunshot_02.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gunshot_02.SetActive(false);


        swat_03.GetComponent<Animation>().Play("Shooting");
        yield return new WaitForSeconds(0.5f);
        gunshot_03.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gunshot_03.SetActive(false);


        swat_04.GetComponent<Animation>().Play("Shooting");
        yield return new WaitForSeconds(0.5f);
        gunshot_04.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gunshot_04.SetActive(false);

        // End Shoot Scene

        camera_04.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        
        swat_01.GetComponent<Animation>().Play("Shooting");
        gunshot_01.SetActive(true);
        shot_01.Play();
        yield return new WaitForSeconds(0.1f);
        gunshot_01.SetActive(false);
        yield return new WaitForSeconds(1);

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
        yield return new WaitForSeconds(2);
        fadeIn.SetActive(true);
        yield return new WaitForSeconds(2);
        fadeIn.SetActive(false);

        playerHUD.SetActive(true);
        cursor.SetActive(false);
        batteryIcon.SetActive(false);
        newspaperBase.SetActive(true);
        newspaper.SetActive(true);
        newspaper.GetComponent<Animation>().Play("RotatingNewspaper");
        newspaperSound.Play();
        yield return new WaitForSeconds(25);
        fadeIn.SetActive(true);
        yield return new WaitForSeconds(2);

        loadingManager.LoadScene(0);
    }
}
