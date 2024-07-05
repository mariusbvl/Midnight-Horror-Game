using System.Collections;
using UnityEngine;

public class ThunderEffect : MonoBehaviour
{
    public GameObject thunderLight;
    public AudioSource thunderSound;
    [SerializeField] private float minDelay = 5f;
    [SerializeField] private float maxDelay = 15f;
    [SerializeField] private float lightDuration = 0.2f;

    private float nextThunderTime;
    private Light lightComponent;

    void Start()
    {
        // Get the Light component attached to the thunderLight GameObject
        lightComponent = thunderLight.GetComponent<Light>();

        if (lightComponent == null)
        {
            Debug.LogError("No Light component found on thunderLight GameObject.");
            return;
        }

        ScheduleNextThunder();
    }

    void Update()
    {
        if (Time.time >= nextThunderTime)
        {
            StartCoroutine(PlayThunderEffect());
            ScheduleNextThunder();
        }
    }

    private void ScheduleNextThunder()
    {
        nextThunderTime = Time.time + Random.Range(minDelay, maxDelay);
    }

    private IEnumerator PlayThunderEffect()
    {
        // Randomize light intensity and color
        lightComponent.intensity = Random.Range(12000f, 15000f);
        lightComponent.color = new Color(Random.Range(0.8f, 1.0f), Random.Range(0.8f, 1.0f), Random.Range(0.8f, 1.0f));

        // Activate the light
        thunderLight.SetActive(true);
        
        // Wait for the light duration
        yield return new WaitForSeconds(lightDuration);

        // Deactivate the light
        thunderLight.SetActive(false);

        // Delay the sound effect to simulate distance
        yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));
        thunderSound.Play();
    }
}
