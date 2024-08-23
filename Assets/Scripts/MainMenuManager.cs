using System;
using FPC;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private GameInputActions _inputActions;
    [SerializeField] private Button startButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject blurredPanel;
    [SerializeField] private GameObject mainObjectsPanel;
    [SerializeField] private Slider cameraSensitivitySlider;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private Button noButton;
    [SerializeField] private GameObject showControlsPanel;
    [SerializeField] private Button showControlsButton;
    private System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> _openExitConfirmationPanelCallback;
    private System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> _closeExitConfirmationPanelCallback;
    private System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> _closeSettingsPanelCallback;
    private System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> _closeShowControlsPanelCallback;
    private void Awake()
    {
        _inputActions = new GameInputActions();
        EventSystem.current.SetSelectedGameObject(startButton.gameObject);

        _openExitConfirmationPanelCallback = _ => OpenExitConfirmationPanel();
        _closeExitConfirmationPanelCallback = _ => CloseExitConfirmationPanel();
        _closeSettingsPanelCallback = _ => CloseOptionsPanel();
        _closeShowControlsPanelCallback = _ => CloseShowControlsPanel();
        _inputActions.Player.Pause.performed += _openExitConfirmationPanelCallback;
    }

    private void Start()
    {
        LoadSettings();
        SaveSettings();
    }

    private void LoadSettings()
    {
        cameraSensitivitySlider.value = SaveManager.Instance.cameraSensitivityValue;
        masterVolumeSlider.value = SaveManager.Instance.masterVolumeValue;
        musicVolumeSlider.value = SaveManager.Instance.musicVolumeValue;
        sfxVolumeSlider.value = SaveManager.Instance.sfxVolumeValue;
    }
    
    private void SaveSettings()
    {
        if (CameraController.Instance != null)
        {
            CameraController.Instance.cameraSensitivity = cameraSensitivitySlider.value;
        }
        SoundMixerManager.Instance.SetMasterVolume(masterVolumeSlider.value);
        SoundMixerManager.Instance.SetSoundFXVolume(sfxVolumeSlider.value);
        SoundMixerManager.Instance.SetMusicVolume(musicVolumeSlider.value);
        SaveManager.Instance.cameraSensitivityValue = cameraSensitivitySlider.value;
        SaveManager.Instance.masterVolumeValue = masterVolumeSlider.value;
        SaveManager.Instance.musicVolumeValue = musicVolumeSlider.value;
        SaveManager.Instance.sfxVolumeValue = sfxVolumeSlider.value;
        SaveManager.Instance.Save();
    }
    
    public void OpenExitConfirmationPanel()
    {
        mainObjectsPanel.SetActive(false);
        confirmationPanel.SetActive(true);
        blurredPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(noButton.gameObject);
        _inputActions.Player.Pause.performed -= _openExitConfirmationPanelCallback;
        _inputActions.Player.Pause.performed += _closeExitConfirmationPanelCallback;
    }
    
    public void CloseExitConfirmationPanel()
    {
        mainObjectsPanel.SetActive(true);
        confirmationPanel.SetActive(false);
        blurredPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(exitButton.gameObject);
        _inputActions.Player.Pause.performed -= _closeExitConfirmationPanelCallback;
        _inputActions.Player.Pause.performed += _openExitConfirmationPanelCallback;
    }
    public void ExitGame()
    {
        Debug.Log("Exited game");
        Application.Quit();    
    }

    public void OpenOptionsPanel()
    {
        mainObjectsPanel.SetActive(false);
        optionsPanel.SetActive(true);
        blurredPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(cameraSensitivitySlider.gameObject);
        _inputActions.Player.Pause.performed -= _openExitConfirmationPanelCallback;
        _inputActions.Player.Pause.performed += _closeSettingsPanelCallback;
    }
    
    public void CloseOptionsPanel()
    {
        optionsPanel.SetActive(false);
        blurredPanel.SetActive(false);
        mainObjectsPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(optionsButton.gameObject);
        _inputActions.Player.Pause.performed -= _closeSettingsPanelCallback;
        _inputActions.Player.Pause.performed += _openExitConfirmationPanelCallback;
        SaveSettings();
    }


    public void OpenShowControlsPanel()
    {
        optionsPanel.SetActive(false);
        showControlsPanel.SetActive(true);
        _inputActions.Player.Pause.performed -= _closeSettingsPanelCallback;
        _inputActions.Player.Pause.performed += _closeShowControlsPanelCallback;
    }

    public void CloseShowControlsPanel()
    {
        optionsPanel.SetActive(true);
        showControlsPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(showControlsButton.gameObject);
        _inputActions.Player.Pause.performed -= _closeShowControlsPanelCallback;
        _inputActions.Player.Pause.performed += _closeSettingsPanelCallback;
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
