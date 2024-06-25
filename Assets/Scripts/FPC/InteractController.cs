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
        private GameObject _currentLockerDoor;
        private GameObject _currentLocker;
        private GameObject _currentDoorPivot;
        private GameObject _currentHideCameraPoint;
        private GameObject _currentFrontOfTheLockerPoint;
        private GameObject _currentExitPointLockerPoint;
        private bool _lockerDoorOnHover;
        //Locker Door Variables
        public float doorOpeningDuration = 0.5f;
        private Quaternion _closedRotation;
        private Quaternion _openRotation;
        private bool _isDoorPositionSet;
        private bool _isLockerDoorOpen;
        //Moving into locker
        private Coroutine _transitionCoroutine;
        public float transitionDuration = 1.0f;
        [Header("SimpleDoor")] 
        private bool _isSimpleDoorOnHover;
        private GameObject _currentSimpleDoor;
        private GameObject _currentSimpleDoorOpenPivot;
        private GameObject _currentSimpleDoorClosedPivot;
        private Quaternion _simpleDoorOpenRotation;
        private Quaternion _simpleDoorCloseRotation;
        private bool _isSimpleDoorOpen;
        [Header("KeyLockedDoor")] 
        [SerializeField] private TMP_Text lockedText;
        private bool _isKeyLockedDoorOnHover;
        private GameObject _currentKeyDoor;
        private TMP_Text _doorIdComponent;
        private string _doorIdText;
        private int _doorIdInt;
        private GameObject _currentKeyDoorOpenPivot;
        private Quaternion _keyDoorOpenRotation;
        [Header("Key")] 
        public bool[] keysPicked;
        private bool _isKeyOnHover;
        private TMP_Text _idTextComponent;
        private string _idText;
        private int _idInt;
        private GameObject _key;
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
                _currentLockerDoor = null;
                
                
                _currentDoorPivot = null;
                _currentLocker = null;
                _currentHideCameraPoint = null;
                _currentFrontOfTheLockerPoint = null;
                _currentExitPointLockerPoint = null;
                _isDoorPositionSet = false;
                _isSimpleDoorOnHover = false;
                _currentSimpleDoor = null;

                _isKeyOnHover = false;
                _key = null;
                
                _currentKeyDoor = null;
                _currentKeyDoorOpenPivot = null;
                _isKeyLockedDoorOnHover = false;
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
                    _currentLockerDoor = rayCastHit.collider.gameObject;
                    _currentDoorPivot = _currentLockerDoor.transform.parent.gameObject;
                    _currentLocker = _currentDoorPivot.transform.parent.gameObject;
                    Transform hideCameraPointTransform = _currentLocker.transform.Find("HideCameraPoint");
                    if (hideCameraPointTransform != null)
                    {
                        _currentHideCameraPoint = hideCameraPointTransform.gameObject;
                    }
                    Transform inFrontOfTheLockerTransform = _currentLocker.transform.Find("InFrontOfTheLockerPoint");
                    if (inFrontOfTheLockerTransform != null)
                    {
                        _currentFrontOfTheLockerPoint = inFrontOfTheLockerTransform.gameObject;
                    }
                    Transform exitLockerPointTransform = _currentLocker.transform.Find("ExitLockerPoint");
                    if (exitLockerPointTransform != null)
                    {
                        _currentExitPointLockerPoint= exitLockerPointTransform.gameObject;
                    }
                    if (_currentDoorPivot != null && !_isDoorPositionSet)
                    {
                        _closedRotation = _currentDoorPivot.transform.rotation;
                        var eulerAngles = _currentDoorPivot.transform.eulerAngles;
                        _openRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y + 90f, eulerAngles.z);
                        _isDoorPositionSet = true;
                    }
                }

                if (rayCastHit.collider.gameObject.CompareTag("SimpleDoor"))
                {
                    _isSimpleDoorOnHover = true;
                    _currentSimpleDoor = rayCastHit.collider.gameObject;
                    _currentDoorPivot = _currentSimpleDoor.transform.parent.gameObject;
                    GameObject simpleDoor = _currentDoorPivot.transform.parent.gameObject;
                    Transform currentSimpleDoorOpenPivotTransform = simpleDoor.transform.Find("openDoor");
                    if (currentSimpleDoorOpenPivotTransform != null)
                    {
                        _currentSimpleDoorOpenPivot= currentSimpleDoorOpenPivotTransform.gameObject;
                    }
                    Transform currentSimpleDoorClosePivotTransform = simpleDoor.transform.Find("closedDoor");
                    if (currentSimpleDoorClosePivotTransform != null)
                    {
                        _currentSimpleDoorClosedPivot= currentSimpleDoorClosePivotTransform.gameObject;
                    }
                    _simpleDoorOpenRotation = _currentSimpleDoorOpenPivot.transform.rotation;
                    _simpleDoorCloseRotation = _currentSimpleDoorClosedPivot.transform.rotation;
                    print("SimpleDoor");
                }

                if (rayCastHit.collider.gameObject.CompareTag("KeyLockedDoor"))
                {
                    _isKeyLockedDoorOnHover = true;
                    _currentKeyDoor = rayCastHit.collider.gameObject;
                    _currentDoorPivot = _currentKeyDoor.transform.parent.gameObject;
                    GameObject keyDoor = _currentDoorPivot.transform.parent.gameObject;
                    Transform currentKeyDoorOpenPivotTransform = keyDoor.transform.Find("openDoor");
                    if (currentKeyDoorOpenPivotTransform != null)
                    {
                        _currentKeyDoorOpenPivot = currentKeyDoorOpenPivotTransform.gameObject;
                    }
                    _keyDoorOpenRotation = _currentKeyDoorOpenPivot.transform.rotation;
                    _doorIdComponent = keyDoor.GetComponentInChildren<TMP_Text>();
                    _doorIdText = _doorIdComponent.text;
                    if (int.TryParse(_doorIdText, out int doorInt))
                    {
                        _doorIdInt = doorInt;
                    }
                    print("LockedDoor");
                }
                if (rayCastHit.collider.gameObject.CompareTag("Key"))
                { 
                    _isKeyOnHover = true;
                    _key = rayCastHit.collider.gameObject;
                    _idTextComponent = _key.GetComponentInChildren<TMP_Text>();
                    _idText = _idTextComponent.text;
                    if (int.TryParse(_idText, out int idInt))
                    {
                        _idInt = idInt;
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
                StartCoroutine(OpenCloseSimpleDoor());
                PickKey();
                StartCoroutine(OpenDoorWithKey());
            }
        }
        
        private IEnumerator OpenDoorWithKey()
        {
            if (_isKeyLockedDoorOnHover)
            {
                Debug.Log($"Door status: {keysPicked[_doorIdInt]}");
                if (keysPicked[_doorIdInt])
                {
                    isInteracting = true;
                    yield return StartCoroutine(OpenKeyDoor());
                    isInteracting = false;
                }
                else
                {
                    yield return StartCoroutine(FadeText(lockedText));
                }
            }
        }
        
        private void PickKey()
        {
            if (_isKeyOnHover)
            {
                keysPicked[_idInt] = true;
                Destroy(_key);
            }
        }
        private IEnumerator OpenCloseSimpleDoor()
        {
            if (_isSimpleDoorOnHover)
            {
                isInteracting = true;
                yield return StartCoroutine(ToggleSimpleDoor());
                isInteracting = false;
            }
        }
        private IEnumerator ExitLocker()
        {
            if (_lockerDoorOnHover && FirstPersonController.Instance.isHiden)
            {
                isInteracting= true;
                player.transform.position = _currentHideCameraPoint.transform.position;
                player.transform.rotation = _currentHideCameraPoint.transform.rotation;
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
                yield return isInteracting = false;
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
                player.transform.position = _currentFrontOfTheLockerPoint.transform.position;
                player.transform.rotation = _currentFrontOfTheLockerPoint.transform.rotation;
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
                ? _currentExitPointLockerPoint.transform.position
                : _currentHideCameraPoint.transform.position;
            Quaternion targetRotation = FirstPersonController.Instance.isHiden
                ? _currentExitPointLockerPoint.transform.rotation
                : _currentHideCameraPoint.transform.rotation;
            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / transitionDuration;
                
                transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
                
                transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);

                yield return null; 
            }
            var transform1 = transform;
            transform1.position = targetPosition;
            transform1.rotation = targetRotation;
        }

        private IEnumerator OpenKeyDoor()
        {
            if (_currentDoorPivot != null)
            {
                yield return StartCoroutine(RotateDoor(_keyDoorOpenRotation));
            }
        }
        
        private IEnumerator ToggleSimpleDoor()
        {
            if (_currentDoorPivot != null)
            {
                yield return StartCoroutine(RotateDoor(_isSimpleDoorOpen ? _simpleDoorCloseRotation : _simpleDoorOpenRotation));
                _isSimpleDoorOpen = !_isSimpleDoorOpen;
            }
        }
        
        private IEnumerator ToggleDoor()
        {
            if (_currentDoorPivot != null)
            {
                yield return StartCoroutine(RotateDoor(_isLockerDoorOpen ? _closedRotation : _openRotation));
                _isLockerDoorOpen = !_isLockerDoorOpen;
            }
        }
        private IEnumerator RotateDoor(Quaternion targetRotation)
        {
            Quaternion startRotation = _currentDoorPivot.transform.rotation;
            float timeElapsed = 0f;

            while (timeElapsed < doorOpeningDuration)
            {
                _currentDoorPivot.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / doorOpeningDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            _currentDoorPivot.transform.rotation = targetRotation; 
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
                StartCoroutine(FadeText(corpsesFoundText));
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
        private IEnumerator FadeText(TMP_Text text)
        {
            text.gameObject.SetActive(true);
            yield return StartCoroutine(Fade(0f, 1f, text));
            yield return new WaitForSeconds(2.0f);
            yield return StartCoroutine(Fade(1f, 0f, text));
            text.gameObject.SetActive(false);
        }

        private IEnumerator Fade(float startAlpha, float endAlpha, TMP_Text text)
        {
            float elapsedTime = 0f;
            Color color = text.color;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
                color.a = alpha;
                text.color = color;
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
