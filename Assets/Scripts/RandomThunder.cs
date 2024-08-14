using System.Collections;
using UnityEngine;

public class RandomThunder : MonoBehaviour
{
    public GameObject[] thunderLights; // Array of thunder lights
    public AudioSource thunderSound;
    [SerializeField] private float minDelay = 5f;
    [SerializeField] private float maxDelay = 15f;
    [SerializeField] private float lightDuration = 0.2f;

    private float nextThunderTime;

    void Start()
    {
        if (thunderLights == null || thunderLights.Length == 0)
        {
            Debug.LogError("No thunder lights assigned.");
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
        // Select a random thunder light from the array
        GameObject selectedLight = thunderLights[Random.Range(0, thunderLights.Length)];

        // Get the Light component attached to the selected thunder light
        Light lightComponent = selectedLight.GetComponent<Light>();
        if (lightComponent == null)
        {
            Debug.LogError("No Light component found on the selected thunder light GameObject.");
            yield break;
        }

        // Randomize light intensity and color
        lightComponent.intensity = Random.Range(20000f, 25000f);
        lightComponent.color = new Color(Random.Range(0.8f, 1.0f), Random.Range(0.8f, 1.0f), Random.Range(0.8f, 1.0f));

        // Activate the selected light
        selectedLight.SetActive(true);

        // Wait for the light duration
        yield return new WaitForSeconds(lightDuration);

        // Deactivate the light
        selectedLight.SetActive(false);

        // Delay the sound effect to simulate distance
        yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));
        thunderSound.Play();
    }
}
