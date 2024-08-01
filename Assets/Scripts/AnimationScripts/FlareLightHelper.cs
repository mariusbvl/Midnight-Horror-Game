using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlareLightHelper : MonoBehaviour
{
    public int LightMode;
    public GameObject FlareLight;

    void Update()
    {
        if (LightMode == 0)
        {
            StartCoroutine(AnimateLight());
        }
    }

    IEnumerator AnimateLight()
    {
        LightMode = Random.Range(1, 4);
        if (LightMode == 1)
        {
            FlareLight.GetComponent<Animation>().Play("FlareLightAnim1");
        }
        if (LightMode == 2)
        {
            FlareLight.GetComponent<Animation>().Play("FlareLightAnim2");
        }
        if (LightMode == 3)
        {
            FlareLight.GetComponent<Animation>().Play("FlareLightAnim3");
        }
        yield return new WaitForSeconds(0.99f);
        LightMode = 0;
    }
}
