using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance { get; private set; }
    [SerializeField] public AudioSource soundFXObject;
    [SerializeField, HideInInspector]public AudioSource audioSource;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    public void PlaySoundFxClip(AudioClip audioClip, Transform spawnTransform, float volume, float spatialBlend, bool isLoop = false, float? givenLength = null)
    {
        audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        audioSource.spatialBlend = spatialBlend;
        audioSource.loop = isLoop;
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, givenLength.HasValue ? givenLength.Value : clipLength);
    } 
}
