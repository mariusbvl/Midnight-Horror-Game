using System.Collections;
using EnemyScripts;
using FPC;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cursor = UnityEngine.Cursor;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] public LoadingManager loadingManagerMainMenu;
    [Header("General")] 
    [HideInInspector]public GameInputActions inputActions;
    public System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> pausePerformedHandler;
    public System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> closeSafeCodePickerHandler;
    public System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> closeElectricBoxHandler;
    public bool isMainGame;
    public bool isEnemyOn;
    [Header("Objective")] 
    [SerializeField] private TMP_Text objectiveTextInGame;
    [SerializeField] private TMP_Text objectiveTextPause;
    private int _currentObjectiveState;
    
    [Header("PausePanel")] 
    [SerializeField] public GameObject playerCanvas;
    [SerializeField]private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    private bool _isPause;
    
    [Header("KeysCollected")] 
    [SerializeField] private TMP_Text[] collectedKeysTxtArray;
    public bool[] collectedKeysBoolArray;
    [SerializeField] private GameObject noKeyFoundText;

    [Header("Timer")] 
    [SerializeField] public int callSeconds;
    [SerializeField] private TMP_Text policeTimerText;

    [Header("ExitHospital")] 
    public bool canExitHospital;
    
    [Header("GameOver")] 
    [SerializeField] private GameObject camHolder;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerMesh;
    [SerializeField] private Transform playerOnJumpScarePoint;
    private bool _onJumpScarePoint;
    [SerializeField] private GameObject enemy;
    [SerializeField] private Transform enemyHeadTransform;
    [SerializeField] private float lookDuration;
    [SerializeField] private GameObject inGameUi;
    [SerializeField] private GameObject hands;
    private bool _jumpScareAnimationAlreadyTriggered;
    [SerializeField]private float gameOverDistance;
    [SerializeField]private GameObject gameOverPanel;
    public FPC.CameraController cameraController;
    private InteractController _interactController;
    private FlashlightAndCameraController _flashlightAndCameraController;
    private static readonly int JumpScare = Animator.StringToHash("jumpScare");
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int IsRunning = Animator.StringToHash("isRunning");

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        inputActions = new GameInputActions();
        //Components
        cameraController = GameObject.Find("Player").GetComponentInChildren<FPC.CameraController>();
        _interactController = GameObject.Find("Player").GetComponent<InteractController>();
        _flashlightAndCameraController = GameObject.Find("Player").GetComponent<FlashlightAndCameraController>();
        //GameOver
        gameOverPanel.SetActive(false);
        //PausePanel
        pausePanel.SetActive(false);
        playerCanvas.SetActive(true);
        //Input
        pausePerformedHandler = _ => TogglePausePanel();
        closeSafeCodePickerHandler = _ => CloseSafeCodePicker();
        closeElectricBoxHandler = _ => CloseElectricBoxPuzzle();
        inputActions.Player.Pause.performed  += pausePerformedHandler;
    }

    private void Start()
    {
        //Objective
        _currentObjectiveState = isMainGame ? 1 : 0;
        ChangeObjective();
        StartCoroutine(InteractController.Instance.FadeText(objectiveTextInGame)); 
    }

    private void Update()
    {
        if (isMainGame && isEnemyOn)
        {
            GameOver();
        }

        if (isMainGame)
        {
            KeysCollectedController();
        }
    }
    
    public void CloseElectricBoxPuzzle()
    {
        if(!InteractController.Instance.isElectricBoxActive) return;
        FirstPersonController.Instance.inputActions.Enable();
        if (ElectricBoxPuzzle.Instance.blinkingCoroutine != null)
        {
            StopCoroutine(ElectricBoxPuzzle.Instance.blinkingCoroutine);
            ElectricBoxPuzzle.Instance.blinkingCoroutine = null;
        }
        if (ElectricBoxPuzzle.Instance.electricBoxTimerCoroutine != null)
        {
            StopCoroutine(ElectricBoxPuzzle.Instance.electricBoxTimerCoroutine);
            ElectricBoxPuzzle.Instance.electricBoxTimerCoroutine = null;
        }
        InteractController.Instance.characterController.enabled = true;
        InteractController.Instance.cameraController.enabled = true;
        hands.SetActive(true);
        _flashlightAndCameraController.enabled = true;
        playerCanvas.SetActive(true);
        InteractController.Instance.electricBoxPanel.SetActive(false);
        InteractController.Instance. electricTimerText.SetActive(false);
        InteractController.Instance.isElectricBoxActive = false;
        Cursor.lockState = CursorLockMode.Locked;
        inputActions.Player.Pause.performed -= closeElectricBoxHandler;
        inputActions.Player.Pause.performed += pausePerformedHandler;
    }
    
    public void CloseSafeCodePicker()
    {
        if (InteractController.Instance.isSafeCodeActive)
        {
            InteractController.Instance.puzzleCanvasBlur.SetActive(false);
            InteractController.Instance.safeCodeImage.SetActive(false);
            InteractController.Instance.characterController.enabled = true;
            cameraController.enabled = true;
            _flashlightAndCameraController.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            playerCanvas.SetActive(true);
            hands.SetActive(true);
            InteractController.Instance.isSafeCodeActive = false;
            inputActions.Player.Pause.performed -= closeSafeCodePickerHandler;
            inputActions.Player.Pause.performed += pausePerformedHandler;
        }
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

    private void CanExitHospital()
    {
        if (!canExitHospital) return;
        _currentObjectiveState = 4;
        ChangeObjective();
        StartCoroutine(InteractController.Instance.FadeText(objectiveTextInGame));
    }
    
    public void StartPoliceTimer()
    {
        _currentObjectiveState = 3;
        ChangeObjective();
        StartCoroutine(InteractController.Instance.FadeText(objectiveTextInGame));
        StartCoroutine(PoliceTimer());
    }
    
    private IEnumerator PoliceTimer()
    {
        policeTimerText.gameObject.SetActive(true);
        float remainingTime = callSeconds;
        while (remainingTime > 0)
        {
            remainingTime--;
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            policeTimerText.text = $"{minutes:00}:{seconds:00}";
            yield return new WaitForSeconds(1f);
        }
        if (remainingTime != 0) yield break;
        policeTimerText.gameObject.SetActive(false);
        canExitHospital = true;
        CanExitHospital();
    }
    public void CheckForAllCorpses()
    {
        if (InteractController.Instance.nrOfCorpses != 5) return;
        if (InteractController.Instance.canCallPolice) return;
        _currentObjectiveState = 2;
        ChangeObjective();
        StartCoroutine(InteractController.Instance.FadeText(objectiveTextInGame));
        InteractController.Instance.canCallPolice = true;
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
            case 5: { objectiveTextPause.text = Objectives.LeaveHospitalTerritory; objectiveTextInGame.text = Objectives.LeaveHospitalTerritory; break;}
        }
    }

    public void TogglePausePanel()
    {
        _isPause = !_isPause;
        if (_isPause)
        {
            Time.timeScale = 0f;
            cameraController.enabled = false;
            _interactController.enabled = false;
            _flashlightAndCameraController.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            playerCanvas.SetActive(false);
            pausePanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
        }
        else
        {
            cameraController.enabled = true;
            _interactController.enabled = true;
            _flashlightAndCameraController.enabled = true;
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            pausePanel.SetActive(false);
            playerCanvas.SetActive(true);
        }
    }
    private void GameOver()
    {
        if (EnemyAI.Instance.isChasing)
        {
            if (Vector3.Distance(player.transform.position, enemy.transform.position) <= gameOverDistance)
            {
                //Turn off any interaction
                FirstPersonController.Instance.inputActions.Disable();
                FirstPersonController.Instance.velocity = new Vector3(0, 0, 0);
                FirstPersonController.Instance.moveSpeed = 0f;
                EnemyAI.Instance.aiNavMesh.speed = 0f;
                EnemyAI.Instance.aiAnimator.SetBool(IsWalking, false);
                EnemyAI.Instance.aiAnimator.SetBool(IsRunning, false);
                playerMesh.SetActive(false);
                cameraController.enabled = false;
                _interactController.enabled = false;
                _flashlightAndCameraController.enabled = false;
                inGameUi.SetActive(false);
                hands.SetActive(false);
                Quaternion enemyLook = Quaternion.LookRotation(player.transform.position - enemy.transform.position);
                enemy.transform.rotation = enemyLook;
                StartCoroutine(GameOverCoroutine());
            }
        }
    }

    private IEnumerator GameOverCoroutine()
    {
        if (!_onJumpScarePoint)
        {
            // Convert local position to world position
            Vector3 worldPosition = enemy.transform.TransformPoint(playerOnJumpScarePoint.localPosition);
            player.transform.position = worldPosition;
            _onJumpScarePoint = true;
        }
        //Rotate camera to look at enemy
        Quaternion initialRotation = camHolder.transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(enemyHeadTransform.position - camHolder.transform.position);
        float elapsedTime = 0f;
        while (elapsedTime < lookDuration)
        {
            camHolder.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / lookDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        camHolder.transform.rotation = targetRotation;

        if (!_jumpScareAnimationAlreadyTriggered)
        {
            EnemyAI.Instance.aiAnimator.SetTrigger(JumpScare);
            _jumpScareAnimationAlreadyTriggered = true;
        }

        yield return new WaitForSeconds(1.5f);
        gameOverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }


    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    
    
    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
