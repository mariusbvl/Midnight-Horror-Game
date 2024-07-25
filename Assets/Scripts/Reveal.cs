using UnityEngine;

[ExecuteInEditMode]
public class Reveal : MonoBehaviour
{
    [SerializeField] Material Mat;
    [SerializeField] Light SpotLight;

    void Update()
    {
        if (Mat != null && SpotLight != null)
        {
            if (!SpotLight.gameObject.activeSelf)
            {
                Mat.SetFloat("_LightEnabled", 0.0f);
                return;
            }
            
            Mat.SetVector("_LightDirection", SpotLight.transform.forward);
            Mat.SetVector("_LightPosition", SpotLight.transform.position);
            Mat.SetFloat("_LightAngle", SpotLight.spotAngle);
            Mat.SetFloat("_LightEnabled", 1.0f);
        }
    }
}