using System.Collections;
using UnityEngine;

public class IntroPlayThunder : MonoBehaviour
{
    public GameObject thunderLight;
    public AudioSource thunderSound;
    [SerializeField] private float delayBeforeThunder = 10f;
    [SerializeField] private float lightDuration = 0.2f;

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

        // Start the coroutine to play thunder after a delay
        StartCoroutine(PlayThunderAfterDelay());
    }

    private IEnumerator PlayThunderAfterDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delayBeforeThunder);

        // Randomize light intensity and color
        lightComponent.intensity = Random.Range(25000f, 30000f);
        lightComponent.color = new Color(Random.Range(0.8f, 1.0f), Random.Range(0.8f, 1.0f), Random.Range(0.8f, 1.0f));

        // Activate the light
        thunderLight.SetActive(true);

        // Wait for the light duration
        yield return new WaitForSeconds(lightDuration);

        // Deactivate the light
        thunderLight.SetActive(false);

        // Play the thunder sound effect
        thunderSound.Play();
    }
}
