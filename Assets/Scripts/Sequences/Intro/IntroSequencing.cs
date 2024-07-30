using System.Collections;
using TMPro;
using UnityEngine;

public class IntroSequencing : MonoBehaviour
{
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


    private void Start()
    {
        StartCoroutine(SequenceBegin());
    }

    IEnumerator SequenceBegin()
    {
        yield return new WaitForSeconds(2);
        yield return new WaitForSeconds(0.5f);
        placeDisplay.SetActive(true);
        placeDisplay.GetComponent<TextMeshProUGUI>().text = "Ravenwood Asylum";
        yield return new WaitForSeconds(1);
        dateDisplay.SetActive(true);
        dateDisplay.GetComponent<TextMeshProUGUI>().text = "October 15, 1978";
        yield return new WaitForSeconds(4);
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
        yield return new WaitForSeconds(6);
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
        yield return new WaitForSeconds(4);
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
    }
}
