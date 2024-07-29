using System.Collections;
using TMPro;
using UnityEngine;

public class IntroSequencing : MonoBehaviour
{
    public GameObject textBox;
    public GameObject dateDisplay;
    public GameObject placeDisplay;

    private void Start()
    {
        StartCoroutine(SequenceBegin());
    }

    IEnumerator SequenceBegin()
    {
        yield return new WaitForSeconds(3);
        placeDisplay.SetActive(true);
        placeDisplay.GetComponent<TextMeshProUGUI>().text = "Ravenwood Asylum";
        yield return new WaitForSeconds(1);
        dateDisplay.SetActive(true);
        dateDisplay.GetComponent<TextMeshProUGUI>().text = "October 15, 1978";
        yield return new WaitForSeconds(4);
        placeDisplay.SetActive(false);
        dateDisplay.SetActive(false);
        yield return new WaitForSeconds(1);

        TextMeshProUGUI textComponent = textBox.GetComponent<TextMeshProUGUI>();

        textComponent.text = "My name is Nathan Scott, and I am an investigative journalist.";
        yield return new WaitForSeconds(4);
        textComponent.text = "Ravenwood Asylum was once a beacon of hope for the mentally ill.";
        yield return new WaitForSeconds(5);
        textComponent.text = "But in the late 1970s, Dr. Sebastian Voss initiated 'Project Insight'.";
        yield return new WaitForSeconds(6);
        textComponent.text = "He believed that extreme isolation and sensory deprivation could unlock hidden potential in the human mind.";
        yield return new WaitForSeconds(6);
        textComponent.text = "Five patients were selected for the experiment.";
        yield return new WaitForSeconds(5);
        textComponent.text = "But they disappeared under mysterious circumstances...";
        yield return new WaitForSeconds(5);
        textComponent.text = "The asylum was abruptly closed, and the truth buried.";
        yield return new WaitForSeconds(5);
        textComponent.text = "Today, I enter the abandoned Ravenwood Asylum...";
        yield return new WaitForSeconds(4);
        textComponent.text = "To uncover the truth behind the missing patients and Dr. Voss's experiments.";
        yield return new WaitForSeconds(6);
        textComponent.text = "";
    }
}
