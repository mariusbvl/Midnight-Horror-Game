using System.Collections;
using UnityEngine;


namespace FPC
{
    public class FirstPersonController : MonoBehaviour
    {
        [HideInInspector]public GameInputActions inputActions;
        private CharacterController _characterController;
        public float moveSpeed;
        [SerializeField] private HeadBobController headBob;
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
        [HideInInspector]public bool isInAir;
        [HideInInspector]public bool isJumpIdle;
        [Header("Sprint")]
        [SerializeField] private float sprintSpeed;
        [SerializeField] private bool isSprinting;
        [Header("Crouch")]
        [SerializeField]private Transform playerMesh;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float crouchSpeed;
        [SerializeField] private float crouchTime;
        [SerializeField] private float standHeight;
        [SerializeField] private float crouchHeight;
        [SerializeField] private Vector3 crouchingCenter;
        [SerializeField] private Vector3 standingCenter;
        [SerializeField] public bool isCrouching;

        private void Awake()
        {
            moveSpeed = walkSpeed;
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
            _characterController.Move(movement * (moveSpeed * Time.deltaTime));
        }

        private void Jump()
        {
            if (move.y > 0 || !inputActions.Player.Move.inProgress)
            {
                if (!isCrouching)
                {
                    isJumpIdle = !inputActions.Player.Move.inProgress;
                    if (inputActions.Player.Jump.triggered && isGrounded && !isInAir)
                    {
                        _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                    }
                }
                if (!isGrounded && !isInAir)
                {
                    isInAir = true;
                    //Debug.Log($"After jump: {isInAir}");
                }
                if (isGrounded && isInAir)
                {
                    isInAir = false;
                    //Debug.Log($"After landing: {isInAir}");
                }
            }
        }
        
        private IEnumerator JumpCooldown()
        {
            
            yield return new WaitForSeconds(2);  
            
        }
        private void Sprint()
        {
            if (move.y > 0)
            {
                if (isSprinting && isGrounded)
                {
                    moveSpeed = sprintSpeed;
                    headBob.amplitude = 0.01f;
                    headBob.frequency = 20f;
                }
                else if (!isSprinting && isGrounded)
                {
                    moveSpeed = walkSpeed;
                    headBob.amplitude = 0.005f;
                    headBob.frequency = 10f;
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
                moveSpeed = crouchSpeed;
            }
            else if (!isSprinting && isCrouching)
            {
                moveSpeed = walkSpeed;
            }
        }
        
        private IEnumerator CrouchStand()
        {
            float timeElapsed = 0;
            //PlayerMesh
            float startPMeshYTransform= isCrouching ? -1f
                : -0.25f;
            float targetPMeshYTransform = isCrouching ? -0.25f : -1f;
            //Camera
            float startCameraYTransform = isCrouching ? 0.75f : 1.1f;
            float targetCameraYTransform = isCrouching ? 1.1f : 0.75f;
            //CharacterController
            float targetHeight = isCrouching ? crouchHeight : standHeight;
            float currentHeight = _characterController.height;
            Vector3 targetCenter = isCrouching ? crouchingCenter : standingCenter;
            Vector3 currentCenter = _characterController.center;
            while (timeElapsed < crouchTime)
            {
                //PlayerMesh
                Vector3 playerMeshPosition = playerMesh.localPosition;
                playerMeshPosition.y = Mathf.Lerp(startPMeshYTransform, targetPMeshYTransform, timeElapsed/crouchTime);
                playerMesh.localPosition = playerMeshPosition;
                //Camera
                Vector3 cameraPosition = cameraTransform.localPosition;
                cameraPosition.y = Mathf.Lerp(startCameraYTransform, targetCameraYTransform, timeElapsed / crouchTime);
                cameraTransform.localPosition = cameraPosition;
                //Character Controller
                _characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed/crouchTime);
                _characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed/crouchTime);
                
                timeElapsed += Time.deltaTime;
                yield return null; 
            }
            //Character Controller
            _characterController.height = targetHeight;
            _characterController.center = targetCenter;
            //Player Mesh
            Vector3 finalMeshPosition = playerMesh.localPosition;
            finalMeshPosition.y = targetPMeshYTransform;
            playerMesh.localPosition = finalMeshPosition;
            //Camera
            Vector3 finalCameraPosition = cameraTransform.localPosition;
            finalCameraPosition.y = targetCameraYTransform;
            cameraTransform.localPosition = finalCameraPosition;
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
