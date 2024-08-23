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
    [SerializeField] private GameObject playerCanvas;
    [SerializeField] private GameObject finalPanel;
    [SerializeField] private Button continueButton;
    private CharacterController _characterController;
    private CameraController _cameraController;
    private InteractController _interactController;
    private FlashlightAndCameraController _flashlightAndCameraController;
    
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
    }

    void Start()
    {
        LoadSettings();
        _currentObjectiveState = 5;
        ChangeObjective();
        StartCoroutine(InteractController.Instance.FadeText(objectiveTextInGame));
    }

    private void LoadSettings()
    {
        FlashlightAndCameraController.Instance.consumeSlider.value = SaveManager.Instance.batterySliderValue;
        InteractController.Instance.nrOfBatteries = SaveManager.Instance.nrOfBatteries;
        CameraController.Instance.cameraSensitivity = SaveManager.Instance.cameraSensitivityValue;
        SoundMixerManager.Instance.SetMasterVolume(SaveManager.Instance.masterVolumeValue);
        SoundMixerManager.Instance.SetSoundFXVolume(SaveManager.Instance.sfxVolumeValue);
        SoundMixerManager.Instance.SetMusicVolume(SaveManager.Instance.musicVolumeValue);
    }
    
    
    private void Update()
    {
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
