using UnityEngine;

namespace FPC
{
    public class FlashlightController : MonoBehaviour
    {
        private GameInputActions _inputActions;
        [SerializeField] private GameObject flashlight;
        private bool _isFlashlightOn;
        void Awake()
        {
            _inputActions = new GameInputActions();
            _isFlashlightOn = false;
            _inputActions.Player.AltInteract.performed += _ => ChangeFlashlightBool();
        }

        // Update is called once per frame
        void Update()
        {
            SwitchFlashlight();
        }

        private void SwitchFlashlight()
        {
            flashlight.SetActive(_isFlashlightOn);
        }

        private void ChangeFlashlightBool()
        {
            _isFlashlightOn = !_isFlashlightOn;
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
}
