using System.Collections;
using UnityEngine;
using TMPro;


namespace FPC
{
    public class InteractController : MonoBehaviour
    {
        public static InteractController Instance { get; private set; }
        [Header("General")] 
        private GameInputActions _inputActions;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float rayCastDistance;
        public bool isInteracting; 
        [Header("Battery")]
        [SerializeField] public TMP_Text batteryText;
        private GameObject _battery;
        private bool _batteryOnHover;
        [HideInInspector] public float nrOfBatteries;
        [Header("Corpse")] 
        [SerializeField]private TMP_Text corpsesFoundText;
        private GameObject _currentCorpse;
        public GameObject[] foundCorpses;
        private float _nrOfCorpses;
        private bool _corpseOnHover;
        public float fadeDuration = 1.0f;

        [Header("HideInLocker")] 
        [SerializeField] private GameObject playerMesh;
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject hands;
        [SerializeField] private GameObject camHolder;
        private CameraController _cameraController;
        private CharacterController _characterController;
        private FlashlightAndCameraController _flashlightAndCameraController;
        private HeadBobController _headBobController;
        public GameObject currentLockerDoor;
        public GameObject currentLocker;
        public GameObject currentDoorPivot;
        public GameObject currentHideCameraPoint;
        public GameObject currentFrontOfTheLockerPoint;
        public GameObject currentExitPointLockerPoint;
        private bool _lockerDoorOnHover;
        //Locker Door Variables
        public float doorOpeningDuration = 0.5f;
        private Quaternion _closedRotation;
        private Quaternion _openRotation;
        private bool _isDoorPositionSet;
        private bool _isOpen;
        //Moving into locker
        private Coroutine _transitionCoroutine;
        public float transitionDuration = 1.0f;
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            _characterController = GetComponent<CharacterController>();
            _cameraController = camHolder.GetComponent<CameraController>();
            _flashlightAndCameraController = GetComponent<FlashlightAndCameraController>();
            _headBobController = GetComponent<HeadBobController>();
            nrOfBatteries = 3;
            batteryText.text = nrOfBatteries + "/5";
            _inputActions = new GameInputActions();
            _inputActions.Player.Interact.performed += _ => Interact();
        }

        
        void Update()
        {
            CursorRayCast();
        }
        private void CursorRayCast(){
            if (!isInteracting)
            {
                _batteryOnHover = false;
                _battery = null;
                _corpseOnHover = false;
                _currentCorpse = null;
                _lockerDoorOnHover = false;
                currentLockerDoor = null;
                currentDoorPivot = null;
                currentLocker = null;
                currentHideCameraPoint = null;
                currentFrontOfTheLockerPoint = null;
                currentExitPointLockerPoint = null;
                _isDoorPositionSet = false;
            }
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var rayCastHit, rayCastDistance))
            {
                if (rayCastHit.collider.gameObject.CompareTag("Battery"))
                {
                    _batteryOnHover = true;
                    _battery = rayCastHit.collider.gameObject;
                }

                if (FlashlightAndCameraController.Instance.recordingImage.activeSelf)
                {
                    rayCastDistance = 3;
                    if (rayCastHit.collider.gameObject.CompareTag("Corpse"))
                    {
                        _corpseOnHover = true;
                        _currentCorpse = rayCastHit.collider.gameObject;
                    }
                }else{rayCastDistance = 1.2f;}
                
                if(rayCastHit.collider.gameObject.CompareTag("LockerDoor"))
                {
                    _lockerDoorOnHover = true;
                    currentLockerDoor = rayCastHit.collider.gameObject;
                    currentDoorPivot = currentLockerDoor.transform.parent.gameObject;
                    currentLocker = currentDoorPivot.transform.parent.gameObject;
                    Transform hideCameraPointTransform = currentLocker.transform.Find("HideCameraPoint");
                    if (hideCameraPointTransform != null)
                    {
                        currentHideCameraPoint = hideCameraPointTransform.gameObject;
                    }
                    Transform inFrontOfTheLockerTransform = currentLocker.transform.Find("InFrontOfTheLockerPoint");
                    if (inFrontOfTheLockerTransform != null)
                    {
                        currentFrontOfTheLockerPoint = inFrontOfTheLockerTransform.gameObject;
                    }
                    Transform exitLockerPointTransform = currentLocker.transform.Find("ExitLockerPoint");
                    if (exitLockerPointTransform != null)
                    {
                        currentExitPointLockerPoint= exitLockerPointTransform.gameObject;
                    }
                    if (currentDoorPivot != null && !_isDoorPositionSet)
                    {
                        _closedRotation = currentDoorPivot.transform.rotation;
                        var eulerAngles = currentDoorPivot.transform.eulerAngles;
                        _openRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y + 90f, eulerAngles.z);
                        _isDoorPositionSet = true;
                    }
                }
            }
        }

        private void Interact()
        {
            if (!isInteracting)
            {
                InteractWithBattery();
                PhotoCorpse();
                StartCoroutine(HideInLocker());
                StartCoroutine(ExitLocker());
            }
        }


        private IEnumerator ExitLocker()
        {
            if (_lockerDoorOnHover && FirstPersonController.Instance.isHiden)
            {
                isInteracting= true;
                player.transform.position = currentHideCameraPoint.transform.position;
                player.transform.rotation = currentHideCameraPoint.transform.rotation;
                yield return StartCoroutine(ToggleDoor());
                yield return StartCoroutine(SmoothTransitionCoroutine());
                yield return _characterController.enabled = true;
                yield return _cameraController.enabled = true;
                _flashlightAndCameraController.enabled = true;
                _headBobController.enabled = true;
                playerMesh.SetActive(true);
                hands.SetActive(true);
                yield return StartCoroutine(ToggleDoor());
                yield return FirstPersonController.Instance.isHiden = false;
                yield return isInteracting= false;
            }
        }
        private IEnumerator HideInLocker()
        {
            if (_lockerDoorOnHover && !FirstPersonController.Instance.isHiden)
            {
                isInteracting = true;
                if (FirstPersonController.Instance.isCrouching)
                {
                    FirstPersonController.Instance.CrouchPressed();
                    AnimationController.Instance.animator.SetTrigger("Stand");
                    AnimationController.Instance.animator.SetBool("isCrouching", false);
                    yield return StartCoroutine(FirstPersonController.Instance.CrouchStand());
                }
                
                _characterController.enabled = false;
                _cameraController.enabled = false;
                _flashlightAndCameraController.enabled = false;
                FlashlightAndCameraController.Instance.isFlashlightOn = false;
                FlashlightAndCameraController.Instance.isCameraOn = false;
                FlashlightAndCameraController.Instance.flashlight.SetActive(false);
                FlashlightAndCameraController.Instance.recordingImage.SetActive(false);
                FlashlightAndCameraController.Instance.ConsumeBattery();
                _headBobController.enabled = false;
                playerMesh.SetActive(false);
                hands.SetActive(false);
                player.transform.position = currentFrontOfTheLockerPoint.transform.position;
                player.transform.rotation = currentFrontOfTheLockerPoint.transform.rotation;
                yield return StartCoroutine(ToggleDoor());

                yield return StartCoroutine(SmoothTransitionCoroutine());

                yield return StartCoroutine(ToggleDoor());
                yield return FirstPersonController.Instance.isHiden = true;
                yield return isInteracting= false;
            }
        }

        private IEnumerator SmoothTransitionCoroutine()
        {
            float elapsedTime = 0f;

            Vector3 initialPosition = player.transform.position;
            Quaternion initialRotation = player.transform.rotation;
            Vector3 targetPosition = FirstPersonController.Instance.isHiden
                ? currentExitPointLockerPoint.transform.position
                : currentHideCameraPoint.transform.position;
            Quaternion targetRotation = FirstPersonController.Instance.isHiden
                ? currentExitPointLockerPoint.transform.rotation
                : currentHideCameraPoint.transform.rotation;
            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / transitionDuration;

                // Smoothly move the position
                transform.position = Vector3.Lerp(initialPosition, targetPosition, t);

                // Smoothly rotate towards the target rotation
                transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);

                yield return null; // Wait until the next frame
            }

            // Ensure the final position and rotation match exactly
            var transform1 = transform;
            transform1.position = targetPosition;
            transform1.rotation = targetRotation;
        }

        private IEnumerator ToggleDoor()
        {
            if (currentDoorPivot != null)
            {
                yield return StartCoroutine(RotateDoor(_isOpen ? _closedRotation : _openRotation));
                _isOpen = !_isOpen;
            }
        }
        private IEnumerator RotateDoor(Quaternion targetRotation)
        {
            Quaternion startRotation = currentDoorPivot.transform.rotation;
            float timeElapsed = 0f;

            while (timeElapsed < doorOpeningDuration)
            {
                currentDoorPivot.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / doorOpeningDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            currentDoorPivot.transform.rotation = targetRotation; // Ensure final rotation is set
        }
        private void InteractWithBattery()
        {
            if (_batteryOnHover)
            {
                nrOfBatteries++;
                batteryText.text = nrOfBatteries + "/5";
                Destroy(_battery);
            }
        }
        
        private void PhotoCorpse()
        {
            if (_corpseOnHover && !IsCorpseAlreadyFound(_currentCorpse))
            {
                AddGameObject(_currentCorpse);
                _nrOfCorpses++;
                corpsesFoundText.text = _nrOfCorpses + "/5";
                StartCoroutine(FadeText());
            }
        }
        private bool IsCorpseAlreadyFound(GameObject corpse)
        {
            foreach (GameObject foundCorpse in foundCorpses)
            {
                if (foundCorpse == corpse)
                {
                    return true;
                }
            }
            return false;
        }

        private void AddGameObject(GameObject obj)
        {
            GameObject[] newArray = new GameObject[foundCorpses.Length + 1];
            for (int i = 0; i < foundCorpses.Length; i++)
            {
                newArray[i] = foundCorpses[i];
            }
            newArray[foundCorpses.Length] = obj;
            foundCorpses = newArray;
        }
        private IEnumerator FadeText()
        {
            corpsesFoundText.gameObject.SetActive(true);
            yield return StartCoroutine(Fade(0f, 1f));
            yield return new WaitForSeconds(2.0f);
            yield return StartCoroutine(Fade(1f, 0f));
            corpsesFoundText.gameObject.SetActive(false);
        }

        private IEnumerator Fade(float startAlpha, float endAlpha)
        {
            float elapsedTime = 0f;
            Color color = corpsesFoundText.color;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
                color.a = alpha;
                corpsesFoundText.color = color;
                yield return null;
            }
            
            color.a = endAlpha;
            corpsesFoundText.color = color;
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
