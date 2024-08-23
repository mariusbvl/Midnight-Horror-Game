using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsJumpscare : MonoBehaviour
{
    public GameObject light_01;
    public GameObject light_02;
    public GameObject light_03;
    public GameObject light_04;
    public GameObject light_05;
    public int flickerCount = 10;

    private void OnTriggerEnter(Collider other)
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
        StartCoroutine(FlickerLights());
    }

    IEnumerator FlickerLights()
    {
        for (int i = 0; i < flickerCount; i++)
        {
            light_01.SetActive(false);
            light_02.SetActive(false);
            light_03.SetActive(false);
            light_04.SetActive(false);
            light_05.SetActive(false);

            yield return new WaitForSeconds(0.3f);

            light_01.SetActive(true);
            light_02.SetActive(true);
            light_03.SetActive(true);
            light_04.SetActive(true);
            light_05.SetActive(true);

            yield return new WaitForSeconds(0.3f);
        }
        light_01.SetActive(false);
        light_02.SetActive(false);
        light_03.SetActive(false);
        light_04.SetActive(false);
        light_05.SetActive(false);
    }
}
