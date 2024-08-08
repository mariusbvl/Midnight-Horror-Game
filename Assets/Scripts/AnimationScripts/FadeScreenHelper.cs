using FPC;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeScreenHelper : MonoBehaviour
{
    public GameObject ThePlayer;
    public GameObject PlayerCanvas;
    public GameObject FadeScreenIn;
    private int _sceneIndex;

    private void Start()
    {
        _sceneIndex = SceneManager.GetActiveScene().buildIndex;
        //if(_sceneIndex != 0) ThePlayer.GetComponent<FirstPersonController>().enabled = false;
        StartCoroutine(FadeScreen());
    }

    IEnumerator FadeScreen()
    {
        yield return new WaitForSeconds(1.5f);
        FadeScreenIn.SetActive(false);
        if (_sceneIndex == 0) yield break;
        PlayerCanvas.SetActive(true);
        //ThePlayer.GetComponent<FirstPersonController>().enabled = true;
    }
}
