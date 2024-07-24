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
            if(!SpotLight.gameObject.activeSelf) return;
            Mat.SetVector("_LightDirection", SpotLight.transform.forward);
            Mat.SetVector("_LightPosition", SpotLight.transform.position);
            Mat.SetFloat("_LightAngle", SpotLight.spotAngle);
        }
    }
}