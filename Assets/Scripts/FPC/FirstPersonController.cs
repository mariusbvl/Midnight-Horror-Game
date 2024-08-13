using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FPC
{
    public class FirstPersonController : MonoBehaviour
    {
        public static FirstPersonController Instance { get; private set; }
        [HideInInspector]public GameInputActions inputActions;
        private CharacterController _characterController;
        public float moveSpeed;
        [SerializeField] private HeadBobController headBob;
        [Header("Move")]
        [SerializeField] private float walkSpeed;
        [HideInInspector] public Vector3 velocity;
        [HideInInspector]public Vector2 move;
        [HideInInspector] public Vector3 movement;
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
        [SerializeField] public bool isSprinting;
        private float _sprintTime;
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
        [Header("Hide")] 
        [SerializeField] public bool isHidden;
        
        [Header("Fall Detection")]
        [SerializeField] private float fallThreshold = 2f;
        [SerializeField] private Volume globalVolume;
        private Vignette _vignette;
        private float _fallTime;
        private bool _isFalling;
        
        [Header("Audio")] 
        [SerializeField] private AudioClip footSteps;
        [SerializeField] private AudioClip breath;
        [SerializeField] private AudioClip fallDamageAudio;
        private float _footstepTimer;
        private float _footstepInterval;
        private AudioSource _breathAudioSource;
        private bool _isBreathCoroutineRunning;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            if (globalVolume.profile.TryGet(out _vignette))
            {
                _vignette.active = true;
            }
            moveSpeed = walkSpeed;
            _footstepInterval = 0.5f;
            _footstepTimer = _footstepInterval;
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
            HandleFootsteps();
            HandleBreathSound();
            HandleFallDetection();
        }

        private void Gravity()
        {
            isGrounded = Physics.CheckSphere(ground.position, distanceToGround, groundMask);
            if (isGrounded && (velocity.y < 0))
            {
                velocity.y = -2f;
            }

            velocity.y += gravity * Time.deltaTime;
            _characterController.Move(velocity * Time.deltaTime);
        }

        private void PlayerMovement()
        {
            move = inputActions.Player.Move.ReadValue<Vector2>();
            var transform1 = transform;
            movement = (move.y * transform1.forward) + (move.x * transform1.right);
            _characterController.Move(movement * (moveSpeed * Time.deltaTime));
        }
        
        private void HandleFallDetection()
        {
            if (!isGrounded && velocity.y < 0)
            {
                if (!_isFalling)
                {
                    _isFalling = true;
                    AnimationController.Instance.animator.SetBool("isFalling", true);
                    _fallTime = 0;
                }
                _fallTime += Time.deltaTime;
            }
            else if (isGrounded && _isFalling)
            {
                _isFalling = false;
                AnimationController.Instance.animator.SetBool("isFalling", false);
                if (_fallTime > fallThreshold)
                {
                    OnLongFallLanded();
                }
                _fallTime = 0;
            }
        }

        private void OnLongFallLanded()
        {
            SoundFXManager.Instance.PlaySoundFxClip(fallDamageAudio, transform , 1f , 0f);
            StartCoroutine(FadeVignette());
            Debug.Log("Player landed after a long fall!");
        }
        
        private IEnumerator FadeVignette()
        {
            float inDuration = 0.5f; 
            float outDuration = 2.0f; 
            float waitTime = 10.0f; 
            
            float initialIntensity = _vignette.intensity.value;
            float initialSmoothness = _vignette.smoothness.value;
            Color initialColor = _vignette.color.value;
            
            float targetIntensity = 1.0f;
            float targetSmoothness = 1.0f;
            Color targetColor = Color.red;
            
            for (float t = 0; t < inDuration; t += Time.deltaTime)
            {
                float normalizedTime = t / inDuration;
                _vignette.intensity.value = Mathf.Lerp(initialIntensity, targetIntensity, normalizedTime);
                _vignette.smoothness.value = Mathf.Lerp(initialSmoothness, targetSmoothness, normalizedTime);
                _vignette.color.value = Color.Lerp(Color.black, targetColor, normalizedTime);
                _vignette.rounded.value = true;
                yield return null;
            }
            
            _vignette.intensity.value = targetIntensity;
            _vignette.smoothness.value = targetSmoothness;
            _vignette.color.value = targetColor;
            _vignette.rounded.value = true;
            
            yield return new WaitForSeconds(waitTime);
            
            for (float t = 0; t < outDuration; t += Time.deltaTime)
            {
                float normalizedTime = t / outDuration;
                _vignette.intensity.value = Mathf.Lerp(targetIntensity, initialIntensity, normalizedTime);
                _vignette.smoothness.value = Mathf.Lerp(targetSmoothness, initialSmoothness, normalizedTime);
                _vignette.color.value = Color.Lerp(targetColor, initialColor, normalizedTime);
                yield return null;
            }
            _vignette.intensity.value = initialIntensity;
            _vignette.smoothness.value = initialSmoothness;
            _vignette.color.value = initialColor;
            _vignette.rounded.value = false;
        }
        
        private void HandleFootsteps()
        {
            if (isGrounded && movement.magnitude > 0)
            {
                _footstepTimer -= Time.deltaTime;
                if (_footstepTimer <= 0)
                {
                    SoundFXManager.Instance.PlaySoundFxClip(footSteps, transform, 0.25f, 0f);
                    _footstepTimer = _footstepInterval;
                }
            }
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
                        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
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
        
        private void Sprint()
        {
            if (move.y > 0)
            {
                if (isSprinting && isGrounded)
                {
                    moveSpeed = sprintSpeed;
                    _footstepInterval = 0.3f;
                    headBob.amplitude = 0.0032f;
                    headBob.frequency = 20f;
                    _sprintTime += Time.deltaTime;
                }
                else if (!isSprinting && isGrounded)
                {
                    moveSpeed = walkSpeed;
                    _footstepInterval = 0.5f;
                    headBob.amplitude = 0.0016f;
                    headBob.frequency = 10f;
                    _sprintTime = 0;
                }
            }
            else
            {
                _sprintTime = 0;
            }
        }
        
        private void HandleBreathSound()
        {
            if (isSprinting && _sprintTime >= 5f)
            {
                if (_breathAudioSource == null || !_breathAudioSource.isPlaying)
                {
                    _breathAudioSource = new GameObject("BreathAudioSource").AddComponent<AudioSource>();
                    _breathAudioSource.transform.position = transform.position;
                    _breathAudioSource.clip = breath;
                    _breathAudioSource.volume = 1f;
                    _breathAudioSource.spatialBlend = 0f;
                    _breathAudioSource.loop = true;
                    _breathAudioSource.Play();
                }
            }
            else if (_breathAudioSource != null && _breathAudioSource.isPlaying)
            {
                if (!_isBreathCoroutineRunning)
                {
                    StartCoroutine(StopBreathSoundAfterDelay(10f));
                }
            }
        }
        
        private IEnumerator StopBreathSoundAfterDelay(float delay)
        {
            _isBreathCoroutineRunning = true;
            yield return new WaitForSeconds(delay);
            if (_breathAudioSource != null)
            {
                _breathAudioSource.Stop();
                Destroy(_breathAudioSource.gameObject);
                _breathAudioSource = null;
            }
            _isBreathCoroutineRunning = false;
        }
        private void Crouch()
        {
            if (!isHidden)
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
        }
        
        public IEnumerator CrouchStand()
        {
            float timeElapsed = 0;
            //PlayerMesh
            float startPMeshYTransform= isCrouching ? -1f
                : -0.25f;
            float targetPMeshYTransform = isCrouching ? -0.15f : -0.94f;
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

        public void CrouchPressed()
        {
            if (!isHidden)
            {
                isCrouching = !isCrouching;
            }
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
