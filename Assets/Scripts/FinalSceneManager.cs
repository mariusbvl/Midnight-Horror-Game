using System;
using FPC;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class FinalSceneManager : MonoBehaviour
{
    private GameInputActions _inputActions;
    [SerializeField] private BoxCollider finalCutSceneTrigger; 
    [SerializeField] private GameObject playerCanvas;
    [SerializeField] private GameObject finalPanel;
    [SerializeField] private Button continueButton;
    private CharacterController _characterController;
    private CameraController _cameraController;
    private InteractController _interactController;
    private FlashlightAndCameraController _flashlightAndCameraController;
    [Header("PausePanel")]
    [SerializeField]private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    private bool _isPause;
    [Header("Objective")] 
    [SerializeField] private TMP_Text objectiveTextInGame;
    [SerializeField] private TMP_Text objectiveTextPause;
    private int _currentObjectiveState;
    private void Awake()
    {
        _inputActions = new GameInputActions();
        finalPanel.SetActive(false);
        _cameraController = FindObjectOfType<CameraController>().GetComponent<CameraController>();
        _flashlightAndCameraController = FindObjectOfType<FlashlightAndCameraController>().GetComponent<FlashlightAndCameraController>();
        _interactController = FindObjectOfType<InteractController>().GetComponent<InteractController>();
        _characterController = FindObjectOfType<CharacterController>().GetComponent<CharacterController>();
        _inputActions.Player.Pause.performed += _ => TogglePausePanel();
    }

    void Start()
    {
        _currentObjectiveState = 5;
        ChangeObjective();
        StartCoroutine(InteractController.Instance.FadeText(objectiveTextInGame));
    }


    private void Update()
    {
       Win();
    }
    
    private void Win(){
        if (_characterController.bounds.Intersects(finalCutSceneTrigger.bounds) && !finalPanel.activeSelf)
        {
            Time.timeScale = 0f;
            _cameraController.enabled = false;
            _interactController.enabled = false;
            _flashlightAndCameraController.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            playerCanvas.SetActive(false);
            finalPanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
        }
    }
    
    public void ChangeObjective()
    {
        switch (_currentObjectiveState)
        {
            case 0: { objectiveTextPause.text = Objectives.EnterHospital; objectiveTextInGame.text = Objectives.EnterHospital; break; }
            case 1: { objectiveTextPause.text = $"{Objectives.FindCorpses} {InteractController.Instance.corpsesFoundText.text}" ; objectiveTextInGame.text = Objectives.FindCorpses; break; }
            case 2: { objectiveTextPause.text = Objectives.CallPolice; objectiveTextInGame.text = Objectives.CallPolice; break; }
            case 3: { objectiveTextPause.text = Objectives.WaitForPolice; objectiveTextInGame.text = Objectives.WaitForPolice; break; }
            case 4: { objectiveTextPause.text = Objectives.ExitHospital; objectiveTextInGame.text = Objectives.ExitHospital; break; }
            case 5: { objectiveTextPause.text = Objectives.LeaveHospitalTerritory; objectiveTextInGame.text = Objectives.LeaveHospitalTerritory; break; }
        }
    }
    
    public void TogglePausePanel()
    {
        _isPause = !_isPause;
        if (_isPause)
        {
            Time.timeScale = 0f;
            _cameraController.enabled = false;
            _interactController.enabled = false;
            _flashlightAndCameraController.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            playerCanvas.SetActive(false);
            pausePanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
        }
        else
        {
            _cameraController.enabled = true;
            _interactController.enabled = true;
            _flashlightAndCameraController.enabled = true;
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            pausePanel.SetActive(false);
            playerCanvas.SetActive(true);
        }
    }
    
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
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
