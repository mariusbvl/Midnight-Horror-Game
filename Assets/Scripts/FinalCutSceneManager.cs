using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalCutSceneManager : MonoBehaviour
{
    [SerializeField] private LoadingManager loadingManager;

    private void Start()
    {
        StartCoroutine(WaitForTimeLineToEnd());
    }

    private IEnumerator WaitForTimeLineToEnd()
    {
        yield return new WaitForSeconds(39f);
        loadingManager.LoadScene(5);
    }
    
}
