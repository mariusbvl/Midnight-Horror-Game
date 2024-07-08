using System.Collections;
using EnemyScripts;
using FPC;
using UnityEngine;
using UnityEngine.SceneManagement;



public class GameManager : MonoBehaviour
{
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
    //private EnemyAI _enemyAI;
    private static readonly int JumpScare = Animator.StringToHash("jumpScare");
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int IsRunning = Animator.StringToHash("isRunning");

    private void Awake()
    {
        //player = GameObject.Find("Player");
        cameraController = GameObject.Find("Player").GetComponentInChildren<FPC.CameraController>();
        _interactController = GameObject.Find("Player").GetComponent<InteractController>();
        _flashlightAndCameraController = GameObject.Find("Player").GetComponent<FlashlightAndCameraController>();
        //_enemyAI = GameObject.Find("Enemy").GetComponent<EnemyAI>();
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
            if (Vector3.Distance(player.transform.position, enemy.transform.position) <=
                gameOverDistance)
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
}
