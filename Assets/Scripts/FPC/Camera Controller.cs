using UnityEngine;

namespace FPC
{
    public class CameraController : MonoBehaviour
    {
        [Header("Camera")]
        [SerializeField] private float cameraSensitivity;
        [SerializeField] private Transform playerBody;
        private Vector2 _mouseLook;
        private float _xRotation;
    
        private GameInputActions _inputActions;
        private void Awake()
        {
            _inputActions = new GameInputActions();
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            Look();
        }

        private void Look()
        {
            _mouseLook = _inputActions.Player.Look.ReadValue<Vector2>();
            float mouseX = _mouseLook.x * cameraSensitivity * Time.deltaTime;
            float mouseY = _mouseLook.y * cameraSensitivity * Time.deltaTime;
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90);
            transform.localRotation = Quaternion.Euler(_xRotation, 0, 0);
            playerBody.Rotate(Vector3.up * mouseX);
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
