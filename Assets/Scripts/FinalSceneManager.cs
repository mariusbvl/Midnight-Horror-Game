using FPC;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalSceneManager : MonoBehaviour
{
    [SerializeField] private BoxCollider finalCutSceneTrigger; 
    [SerializeField] private GameObject playerCanvas;
    [SerializeField] private GameObject finalPanel;
    [SerializeField] private Button continueButton;
    private CharacterController _characterController;
    private CameraController _cameraController;
    private InteractController _interactController;
    private FlashlightAndCameraController _flashlightAndCameraController;
    void Start()
    {
        finalPanel.SetActive(false);
        _cameraController = FindObjectOfType<CameraController>().GetComponent<CameraController>();
        _flashlightAndCameraController = FindObjectOfType<FlashlightAndCameraController>().GetComponent<FlashlightAndCameraController>();
        _interactController = FindObjectOfType<InteractController>().GetComponent<InteractController>();
        _characterController = FindObjectOfType<CharacterController>().GetComponent<CharacterController>();
    }

    
    void Update()
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
            Debug.Log("GameEnd");
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
