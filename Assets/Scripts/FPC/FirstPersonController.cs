using System.Collections;
using UnityEngine;

namespace FPC
{
    public class FirstPersonController : MonoBehaviour
    {
        [HideInInspector]public GameInputActions inputActions;
        private CharacterController _characterController;
        public float _moveSpeed;
        [Header("Move")]
        [SerializeField] private float walkSpeed;
        private Vector3 _velocity;
        [HideInInspector]public Vector2 move;
        [Header("Jump")]
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float jumpHeight = 2f;
        [SerializeField] private Transform ground;
        [SerializeField] private float distanceToGround = 0.4f;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] public bool isGrounded;
        [HideInInspector]public bool canJump;
        [Header("Sprint")]
        [SerializeField] private float sprintSpeed;
        [SerializeField] private bool isSprinting;
        [Header("Crouch")]
        [SerializeField] private float crouchSpeed;
        [SerializeField] private float crouchTime;
        [SerializeField] private float standHeight;
        [SerializeField] private float crouchHeight;
        [SerializeField] private Vector3 crouchingCenter;
        [SerializeField] private Vector3 standingCenter;
        [SerializeField] public bool isCrouching;

        private void Awake()
        {
            _moveSpeed = walkSpeed;
            canJump = true;
            inputActions = new GameInputActions();
            _characterController = GetComponent<CharacterController>();
            inputActions.Player.Sprint.performed += _ => SprintPressed();
            inputActions.Player.Sprint.canceled += _ => SprintReleased();
            inputActions.Player.Crouch.performed += _ => CrouchPressed();
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

            _velocity.y += gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }

        private void PlayerMovement()
        {
            move = inputActions.Player.Move.ReadValue<Vector2>();
            var transform1 = transform;
            Vector3 movement = (move.y * transform1.forward) + (move.x * transform1.right);
            _characterController.Move(movement * (_moveSpeed * Time.deltaTime));
        }

        private void Jump()
        {
            if (!isCrouching)
            {
                if (inputActions.Player.Jump.triggered && isGrounded && canJump)
                {
                    _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                    StartCoroutine(JumpCooldown());
                }
            }
        }
        
        private IEnumerator JumpCooldown()
        {
            canJump = false;  
            yield return new WaitForSeconds(2);  
            canJump = true;  
        }
        private void Sprint()
        {
            if (move.y > 0)
            {
                if (isSprinting && isGrounded)
                {
                    _moveSpeed = sprintSpeed;
                }
                else if (!isSprinting && isGrounded)
                {
                        _moveSpeed = walkSpeed;
                }
            }
            else
            {
                //_moveSpeed = walkSpeed;
                //isSprinting = false;
            }
        }

        private void Crouch()
        {
            if (isGrounded)
            {
                if (inputActions.Player.Crouch.triggered && isCrouching)
                {
                    StartCoroutine(CrouchStand());
                }
            }
            if (inputActions.Player.Crouch.triggered && !isCrouching)
            {
                StartCoroutine(CrouchStand());
            }

            if (isCrouching)
            {
                _moveSpeed = crouchSpeed;
            }
            else if (!isSprinting && isCrouching)
            {
                _moveSpeed = walkSpeed;
            }
        }
        
        private IEnumerator CrouchStand()
        {
            float timeElapsed = 0;
            float targetHeight = isCrouching ? crouchHeight : standHeight;
            float currentHeight = _characterController.height;
            Vector3 targetCenter = isCrouching ? crouchingCenter : standingCenter;
            Vector3 currentCenter = _characterController.center;
            while (timeElapsed < crouchTime)
            {
                _characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed/crouchTime);
                _characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed/crouchTime);
                timeElapsed += Time.deltaTime;
                yield return null; 
            }
            _characterController.height = targetHeight;
            _characterController.center = targetCenter;
        }
        
        private void SprintPressed()
        {
            if (isGrounded)
            {
                isSprinting = true;
            }
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
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }
    }
}
