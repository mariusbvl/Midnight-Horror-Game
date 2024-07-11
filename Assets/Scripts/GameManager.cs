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
    [Header("General")] 
    private GameInputActions _inputActions;
    public bool isMainGame;
    public bool isEnemyOn;
    [Header("Objective")] 
    [SerializeField] private TMP_Text objectiveTextInGame;
    [SerializeField] private TMP_Text objectiveTextPause;
    private int _currentObjectiveState;
    
    [Header("PausePanel")] 
    [SerializeField] private GameObject playerCanvas;
    [SerializeField]private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    private bool _isPause;

    [Header("KeysCollected")] 
    [SerializeField] private TMP_Text[] collectedKeysTxtArray;
    public bool[] _collectedKeysBoolArray;
    [SerializeField] private GameObject noKeyFoundText;
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
        _inputActions = new GameInputActions();
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
        _inputActions.Player.Pause.performed += _ => TogglePausePanel();
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

    private void CombineKeys()
    {
        int length = InteractController.Instance.keysPicked.Length;
        _collectedKeysBoolArray = new bool[length + 1];
        for (int i = 0; i < length; i++)
        {
            _collectedKeysBoolArray[i] = InteractController.Instance.keysPicked[i];
        }
        _collectedKeysBoolArray[length] = InteractController.Instance.isKeyCardPicked;
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
            case 3: { objectiveTextPause.text = Objectives.WaitForPolice; objectiveTextInGame.text = Objectives.CallPolice; break; }
            case 4: { objectiveTextPause.text = Objectives.ExitHospital; objectiveTextInGame.text = Objectives.ExitHospital; break; }
            case 5: { objectiveTextPause.text = Objectives.LeaveHospitalTerritory; objectiveTextPause.text = Objectives.LeaveHospitalTerritory; break; }
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
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }
}
