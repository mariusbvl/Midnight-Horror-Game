using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class LoadingManagerMainMenu : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject loadingCanvas;
    public Slider progressSlider;
    public TMP_Text pressAnyKeyText;
    public Image pressAnyKeyIcon;
    public TMP_Text loadingText;
    private void Start()
    {
        pressAnyKeyText.color = new Color(pressAnyKeyText.color.r, pressAnyKeyText.color.g, pressAnyKeyText.color.b, 0);
        pressAnyKeyIcon.color = new Color(pressAnyKeyIcon.color.r, pressAnyKeyIcon.color.g, pressAnyKeyIcon.color.b, 0);
    }

    public void LoadScene(int index)
    {
        Time.timeScale = 1;
        StartCoroutine(LoadScene_Coroutine(index));
    }

    public IEnumerator LoadScene_Coroutine(int index)
    {
        progressSlider.value = 0;
        mainMenuCanvas.SetActive(false);
        loadingCanvas.SetActive(true);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index);
        asyncOperation.allowSceneActivation = false;
        float progress = 0;

        while (!asyncOperation.isDone)
        {
            progress = Mathf.MoveTowards(progress, asyncOperation.progress, Time.deltaTime);
            progressSlider.value = progress;
            if (progress >= 0.9f)
            {
                progressSlider.value = 1;
                loadingText.text = "Done!";
                StartCoroutine(AnimateTextAndImage());
                if (Input.anyKeyDown)
                {
                    StopCoroutine(AnimateTextAndImage());
                    asyncOperation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }

    IEnumerator AnimateTextAndImage()
    {
        float frequency = 2f; // Adjust this value to control the speed of the blinking
        float offset = 0.5f; // Adjust this value to control the phase of the blinking

        while (true)
        {
            float alpha = Mathf.Abs(Mathf.Sin(Time.time * frequency + offset));

            pressAnyKeyText.color = new Color(pressAnyKeyText.color.r, pressAnyKeyText.color.g, pressAnyKeyText.color.b, alpha);
            pressAnyKeyIcon.color = new Color(pressAnyKeyIcon.color.r, pressAnyKeyIcon.color.g, pressAnyKeyIcon.color.b, alpha);

            yield return null;
        }
    }
}