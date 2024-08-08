using System;
using FPC;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class OutsideGameManager : MonoBehaviour
{
    public static OutsideGameManager Instance { get; private set; }
    private GameInputActions _inputActions;
    [SerializeField] public LoadingManager loadingManagerMainMenu;
    [Header("PausePanel")]
    [SerializeField] private GameObject playerCanvas;
    [SerializeField]private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    private bool _isPause;
    private FPC.CameraController _cameraController;
    private InteractController _interactController;
    private FlashlightAndCameraController _flashlightAndCameraController;
    [Header("Objective")] 
    [SerializeField] private TMP_Text objectiveTextInGame;
    [SerializeField] private TMP_Text objectiveTextPause;
    private int _currentObjectiveState;
    [Header("KeysCollected")] 
    [SerializeField] private TMP_Text[] collectedKeysTxtArray;
    public bool[] collectedKeysBoolArray;
    [SerializeField] private GameObject noKeyFoundText;
    
    [Header("PausePanel")]
    [SerializeField] private GameObject mainObjectsPanel;
    [SerializeField] private Button settingsButton;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Slider cameraSensitivitySlider;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private Button noButton;
    [SerializeField] private GameObject showControlsPanel;
    [SerializeField] private Button showControlsButton;
    private Action<UnityEngine.InputSystem.InputAction.CallbackContext> _togglePausePanelCallback;
    private Action<UnityEngine.InputSystem.InputAction.CallbackContext> _closeExitConfirmationPanelCallback;
    private Action<UnityEngine.InputSystem.InputAction.CallbackContext> _closeSettingsPanelCallback;
    private Action<UnityEngine.InputSystem.InputAction.CallbackContext> _closeShowControlsPanelCallback;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        _inputActions = new GameInputActions();
        _cameraController = FindObjectOfType<FPC.CameraController>().GetComponentInChildren<FPC.CameraController>();
        _interactController = FindObjectOfType<InteractController>().GetComponent<InteractController>();
        _flashlightAndCameraController = FindObjectOfType<FlashlightAndCameraController>().GetComponent<FlashlightAndCameraController>();
        _togglePausePanelCallback = _ => TogglePausePanel();
        _closeExitConfirmationPanelCallback = _ => CloseExitConfirmationPanel();
        _closeSettingsPanelCallback = _ => CloseOptionsPanel();
        _closeShowControlsPanelCallback = _ => CloseShowControlsPanel();
        _inputActions.Player.Pause.performed += _togglePausePanelCallback;
    }

    private void Update()
    {
        KeysCollectedController();
    }

    private void Start()
    {
        _currentObjectiveState = 0;
        ChangeObjective();
        StartCoroutine(InteractController.Instance.FadeText(objectiveTextInGame));
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
    
    public void OpenExitConfirmationPanel()
    {
        mainObjectsPanel.SetActive(false);
        confirmationPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(noButton.gameObject);
        _inputActions.Player.Pause.performed -= _togglePausePanelCallback;
        _inputActions.Player.Pause.performed += _closeExitConfirmationPanelCallback;
    }
    
    public void CloseExitConfirmationPanel()
    {
        mainObjectsPanel.SetActive(true);
        confirmationPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(exitButton.gameObject);
        _inputActions.Player.Pause.performed -= _closeExitConfirmationPanelCallback;
        _inputActions.Player.Pause.performed += _togglePausePanelCallback;
    }
    
    public void OpenOptionsPanel()
    {
        mainObjectsPanel.SetActive(false);
        optionsPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(cameraSensitivitySlider.gameObject);
        _inputActions.Player.Pause.performed -= _togglePausePanelCallback;
        _inputActions.Player.Pause.performed += _closeSettingsPanelCallback;
    }
    
    public void CloseOptionsPanel()
    {
        optionsPanel.SetActive(false);
        mainObjectsPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(settingsButton.gameObject);
        _inputActions.Player.Pause.performed -= _closeSettingsPanelCallback;
        _inputActions.Player.Pause.performed += _togglePausePanelCallback;
    }
    
    
    
    private void CombineKeys()
    {
        int length = InteractController.Instance.keysPicked.Length;
        collectedKeysBoolArray = new bool[length + 1];
        for (int i = 0; i < length; i++)
        {
            collectedKeysBoolArray[i] = InteractController.Instance.keysPicked[i];
        }
        collectedKeysBoolArray[length] = InteractController.Instance.isKeyCardPicked;
    }
    private void KeysCollectedController()
    {
        if (InteractController.Instance.aKeyWasPicked)
        {
            CombineKeys();
            InteractController.Instance.aKeyWasPicked = false;
            foreach (var t in collectedKeysTxtArray)
            {
                if (InteractController.Instance.canAddKey && t.text == "")
                {
                    InteractController.Instance.canAddKey = false;
                    t.text = InteractController.Instance.lastPickedKey.name;
                }
            }
            if(collectedKeysTxtArray[0].text != ""){noKeyFoundText.SetActive(false);}
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
    
    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }
}
