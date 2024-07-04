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
    [SerializeField] private Transform playerOnJumpscarePoint;
    private bool onJumpscarePoint;
    [SerializeField] private GameObject enemy;
    [SerializeField] private Transform enemyHeadTransform;
    [SerializeField] private float lookDuration;
    [SerializeField] private GameObject inGameUi;
    [SerializeField] private GameObject hands;
    private bool _jumpscareAnimationAlreadyTriggered;
    [SerializeField]private float gameOverDistance;
    [SerializeField]private GameObject gameOverPanel;
    private FPC.CameraController _cameraController;
    private InteractController _interactController;
    private FlashlightAndCameraController _flashlightAndCameraController;
    private EnemyAI _enemyAI;
    private static readonly int Jumpscare = Animator.StringToHash("jumpscare");

    private void Awake()
    {
        //player = GameObject.Find("Player");
        _cameraController = GameObject.Find("Player").GetComponentInChildren<FPC.CameraController>();
        _interactController = GameObject.Find("Player").GetComponent<InteractController>();
        _flashlightAndCameraController = GameObject.Find("Player").GetComponent<FlashlightAndCameraController>();
        _enemyAI = GameObject.Find("Enemy").GetComponent<EnemyAI>();
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
                EnemyAI.Instance.aiAnimator.SetBool("isWalking", false);
                EnemyAI.Instance.aiAnimator.SetBool("isRunning", false);
                playerMesh.SetActive(false);
                _cameraController.enabled = false;
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
        if (!onJumpscarePoint)
        {
            // Convert local position to world position
            Vector3 worldPosition = enemy.transform.TransformPoint(playerOnJumpscarePoint.localPosition);
            player.transform.position = worldPosition;
            onJumpscarePoint = true;
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

        if (!_jumpscareAnimationAlreadyTriggered)
        {
            EnemyAI.Instance.aiAnimator.SetTrigger(Jumpscare);
            _jumpscareAnimationAlreadyTriggered = true;
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
