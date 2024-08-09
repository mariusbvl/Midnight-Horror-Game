using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceLights : MonoBehaviour
{
    public GameObject redLight;
    public GameObject blueLight;

    void Start()
    {
        StartCoroutine(PlayPoliceLights());
    }

    IEnumerator PlayPoliceLights()
    {
        while (true)
        {
            redLight.SetActive(false);
            blueLight.SetActive(true);
            yield return new WaitForSeconds(1);

            blueLight.SetActive(false);
            redLight.SetActive(true);
            yield return new WaitForSeconds(1);
        }

    }
}
