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
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private Button noButton;
    [SerializeField] private Button yesButton;
    private void Awake()
    {
        _inputActions = new GameInputActions();
        EventSystem.current.SetSelectedGameObject(startButton.gameObject);
    }

    public void OpenExitConfirmationPanel()
    {
        mainObjectsPanel.SetActive(false);
        confirmationPanel.SetActive(true);
        blurredPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(noButton.gameObject);
    }
    
    public void CloseExitConfirmationPanel()
    {
        mainObjectsPanel.SetActive(true);
        confirmationPanel.SetActive(false);
        blurredPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(exitButton.gameObject);
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
    }
    
    public void CloseOptionsPanel()
    {
        optionsPanel.SetActive(false);
        blurredPanel.SetActive(false);
        mainObjectsPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(optionsButton.gameObject);
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
