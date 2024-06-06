using UnityEngine;
namespace FPC
{
    public class FirstPersonController : MonoBehaviour
    {
        private GameInputActions _inputActions;
        [Header("Camera")]
        [SerializeField] private Transform mainCamera;
        [SerializeField] private Transform cameraStand;
        [SerializeField] private Transform cameraCrouch;
        [SerializeField] private float crouchingDuration = 2f;
        [Header("Player")]
        [SerializeField]private float moveSpeed = 6f;
        [SerializeField] private float sprintSpeed = 10f;
        private Vector3 _velocity;
        private float _gravity = -9.81f;
        private Vector2 _move;
        private float _jumpHeight = 2f;
        private CharacterController _characterController;
        public Transform ground;
        public float distanceToGround = 0.4f;
        public LayerMask groundMask;
        public bool isGrounded;
        public bool isSprinting;
        public bool isCrouching;

        private void Awake()
        {
            _inputActions = new GameInputActions();
            _characterController = GetComponent<CharacterController>();

            _inputActions.Player.Sprint.performed += x => SprintdPressed();
            _inputActions.Player.Sprint.canceled += x => SprintReleased();
            _inputActions.Player.Crouch.performed += x => CrouchPressed();
        }

        private void Update()
        {
            Gravity();
            PlayerMovement();
            Sprint();
            Jump();
            Crouch();
        }

        private void Gravity()
        {
            isGrounded = Physics.CheckSphere(ground.position, distanceToGround, groundMask);
            if (isGrounded && (_velocity.y < 0))
            {
                _velocity.y = -2f;
            }

            _velocity.y += _gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }

        private void PlayerMovement()
        {
            _move = _inputActions.Player.Move.ReadValue<Vector2>();
            Vector3 movement = (_move.y * transform.forward) + (_move.x * transform.right);
            _characterController.Move(movement * moveSpeed * Time.deltaTime);
        }

        private void Jump()
        {
            if (!isCrouching)
            {
                if (_inputActions.Player.Jump.triggered && isGrounded)
                {
                    _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
                }
            }
        }

        private void Sprint()
        {
            if (isSprinting)
            {
                moveSpeed = sprintSpeed;
            }
            else
            {
                moveSpeed = 6f;
            }
        }

        private void Crouch()
        {
            if (isCrouching)
            {
                var center = _characterController.center;
                center.y = -0.5f;
                _characterController.height = 1f;
                _characterController.center = center;
                mainCamera.position = Vector3.Lerp(mainCamera.position, cameraCrouch.position, crouchingDuration * Time.deltaTime);
            }
            else if(!isCrouching && isGrounded)
            {
                var center = _characterController.center;
                center.y = 0f;
                _characterController.height = 2f;
                _characterController.center = center;
                mainCamera.position = Vector3.Lerp(mainCamera.position, cameraStand.position, crouchingDuration * Time.deltaTime);
            }
        }
        private void SprintdPressed()
        {
            isSprinting = true;
        }

        private void SprintReleased()
        {
            isSprinting = false;
        }

        private void CrouchPressed()
        {
            isCrouching = !isCrouching;
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
