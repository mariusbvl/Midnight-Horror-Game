using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class IntroSequencing : MonoBehaviour
{
    private GameInputActions _inputActions;
    
    [SerializeField] private LoadingManager loadingManager;
    [SerializeField] private GameObject voiceovers;
    [SerializeField] private GameObject skipTextAndImage;
    [SerializeField] private TMP_Text skipText;
    [SerializeField] private Image skipImage;
    [SerializeField] private Slider skipSlider;
    [SerializeField] private float timeToSkip;
    [SerializeField] private float fadeDuration;
    private bool _isSkipping;
    private System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> _skipCutSceneCallback;
    private System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> _cutSceneResetSliderCallback;
    
    private Coroutine _sequenceCoroutine;
    
    public GameObject textBox;
    public GameObject dateDisplay;
    public GameObject placeDisplay;
    public GameObject thunderLight;
    public GameObject fadeOutScreen;

    public AudioSource line01;
    public AudioSource line02;
    public AudioSource line03;
    public AudioSource line04;
    public AudioSource line05;
    public AudioSource line06;
    public AudioSource line07;
    public AudioSource line08;
    public AudioSource line09;

    public AudioSource wolfSound;
    public AudioSource thunderSound;

    private void Awake()
    {
        _inputActions = new GameInputActions();
        _skipCutSceneCallback = _ => StartCoroutine(SkipCutScene());
        _cutSceneResetSliderCallback = _ => ResetSkippingSlider();
        _inputActions.UI.Submit.performed += _skipCutSceneCallback;
        _inputActions.UI.Submit.canceled += _cutSceneResetSliderCallback;
    }

    private void Start()
    {
        skipSlider.gameObject.SetActive(false);
        _sequenceCoroutine = StartCoroutine(SequenceBegin());
    }

    private void Update()
    {
        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Return) && !_isSkipping)
        {
            StartCoroutine(FadeGraphic(skipText));
            StartCoroutine(FadeGraphic(skipImage));
        }
    }

    private IEnumerator SkipCutScene()
    {
        _isSkipping = true;
        skipTextAndImage.SetActive(false);
        skipSlider.gameObject.SetActive(true);
        float elapsedTime = 0f; 
        skipSlider.value = 0f; 
        while (elapsedTime < timeToSkip)
        {
            if(!_isSkipping) yield break;
            elapsedTime += Time.deltaTime;
            skipSlider.value = Mathf.Lerp(0f, 100f, elapsedTime / timeToSkip);
            yield return null;
        }

        skipSlider.value = 100f;
        voiceovers.SetActive(false);
        StopCoroutine(_sequenceCoroutine);
        loadingManager.LoadScene(2);
    }

    private void ResetSkippingSlider()
    {
        StopCoroutine(SkipCutScene());
        skipSlider.gameObject.SetActive(false);
        skipTextAndImage.SetActive(true);
        _isSkipping = false;
        skipSlider.value = 0f; 
    }
    
    IEnumerator SequenceBegin()
    {
        yield return new WaitForSeconds(2);
        yield return new WaitForSeconds(0.5f);
        placeDisplay.SetActive(true);
        placeDisplay.GetComponent<TextMeshProUGUI>().text = "Ravenwood Asylum";
        yield return new WaitForSeconds(1f);
        dateDisplay.SetActive(true);
        dateDisplay.GetComponent<TextMeshProUGUI>().text = "October 15, 1978";
        yield return new WaitForSeconds(4f);
        placeDisplay.SetActive(false);
        dateDisplay.SetActive(false);

        TextMeshProUGUI textComponent = textBox.GetComponent<TextMeshProUGUI>();
        line01.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.6f);
        textComponent.text = "My name is Nathan Scott, and I am an investigative journalist.";
        yield return new WaitForSeconds(4);
        line02.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.5f);
        textComponent.text = "Ravenwood Asylum was once a beacon of hope for the mentally ill.";
        yield return new WaitForSeconds(3.5f);
        line03.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.2f);
        textComponent.text = "But in the late 1970s, Dr. Sebastian Voss initiated 'Project Insight'.";
        wolfSound.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(6f);
        line04.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.5f);
        textComponent.text = "He believed that extreme isolation and sensory deprivation could unlock hidden potential in the human mind.";
        yield return new WaitForSeconds(7.5f);
        line05.GetComponent<AudioSource>().Play();
        textComponent.text = "Five patients were selected for the experiment.";
        yield return new WaitForSeconds(2.5f);
        line06.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.8f);
        textComponent.text = "But they disappeared under mysterious circumstances...";
        yield return new WaitForSeconds(3);
        line07.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.5f);
        textComponent.text = "The asylum was abruptly closed, and the truth buried.";
        yield return new WaitForSeconds(4f);
        line08.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.5f);
        textComponent.text = "Today, I enter the abandoned Ravenwood Asylum...";
        yield return new WaitForSeconds(3.5f);
        line09.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.2f);
        textComponent.text = "To uncover the truth behind the missing patients and Dr. Voss's experiments.";
        yield return new WaitForSeconds(6);
        thunderSound.GetComponent<AudioSource>().Play();
        thunderLight.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        thunderLight.SetActive(false);
        textComponent.text = "";
        yield return new WaitForSeconds(0.5f);
        fadeOutScreen.SetActive(true);
        yield return new WaitForSeconds(2f);
        loadingManager.LoadScene(2);
    }
    
    public IEnumerator FadeGraphic(Graphic graphic)
    {
        if (graphic.gameObject.activeSelf) yield break;
        graphic.gameObject.SetActive(true);
        yield return StartCoroutine(Fade(0f, 1f, graphic));
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(Fade(1f, 0f, graphic));
        graphic.gameObject.SetActive(false);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, Graphic graphic)
    {
        float elapsedTime = 0f;
        Color color = graphic.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            color.a = alpha;
            graphic.color = color;
            yield return null;
        }

        color.a = endAlpha;
        graphic.color = color;
    }
    
    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }
}
