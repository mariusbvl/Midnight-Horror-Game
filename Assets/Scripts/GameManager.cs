using System;
using System.Collections;
using System.Collections.Generic;
using EnemyScripts;
using FPC;
using UnityEngine;
using CameraController = TMPro.Examples.CameraController;

public class GameManager : MonoBehaviour
{
    [Header("GameOver")] 
    [SerializeField] private GameObject camHolder;
    [SerializeField] private GameObject enemy;
    private Transform enemyHeadTransform;
    private bool _isGoCoroutineRunning;
    [SerializeField]private float gameOverDistance;
    [SerializeField]private GameObject gameOverPanel;
    private CameraController _cameraController;
    private InteractController _interactController;
    private FlashlightAndCameraController _flashlightAndCameraController;

    private void Awake()
    {
        _cameraController = GameObject.Find("CamHolder").GetComponent<CameraController>();
        _interactController = GameObject.Find("Player").GetComponent<InteractController>();
        _flashlightAndCameraController = GameObject.Find("Player").GetComponent<FlashlightAndCameraController>();
        gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        GameOver();
    }

    private void GameOver()
    {
        if (EnemyAI.Instance.isChasing)
        {
            if (Vector3.Distance(gameObject.transform.position, enemy.transform.position) <=
                gameOverDistance)
            {
                StartCoroutine(GameOverCoroutine());
            }
        }
    }

    private IEnumerator GameOverCoroutine()
    {
        _isGoCoroutineRunning = true;
        _cameraController.enabled = false;
        _interactController.enabled = false;
        _flashlightAndCameraController.enabled = false;
        _isGoCoroutineRunning = false;
        if (!_isGoCoroutineRunning)
        {
            gameOverPanel.SetActive(true);
        }
        yield return null;
    }
}
