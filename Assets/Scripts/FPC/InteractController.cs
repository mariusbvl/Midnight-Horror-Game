using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


namespace FPC
{
    public class InteractController : MonoBehaviour
    {
        public static InteractController Instance { get; private set; }
        [Header("General")] 
        private GameInputActions _inputActions;
        [SerializeField] private Camera mainCamera;
        [SerializeField] public float rayCastDistance;
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
        [HideInInspector]public GameObject currentFrontOfTheLockerPoint;
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
        private bool _isDoorOpening;
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
        [Header("Front Door")] 
        [SerializeField] private int mainGameSceneId;
        private bool _isFrontDoorOnHover;
        [Header("Ladder")] 
        private bool _isLadderOnHover;
        private bool _isOnUpperFloor;
        private GameObject _ladder;
        private Transform _bottomPoint;
        private Transform _upPoint;
        [Header("KeyCard")] 
        [SerializeField]private bool isKeyCardPicked;
        private bool _isKeyCardOnHover;
        private GameObject _keyCard;
        [Header("CardDoubleDoor")] 
        [SerializeField]public float cardDoorOpeningDuration;
        [SerializeField] private TMP_Text accessText; 
        private bool _isCardReaderOnHover;
        private GameObject _leftCardDoor;
        private GameObject _rightCardDoor;
        private Transform _leftCardDoorOpenPoint;
        private Transform _rightCardDoorOpenPoint;
        [Header("Rope")] 
        private bool _isRopeOnHover;
        private bool _isInPit;
        private GameObject _rope;
        private Transform _pitTopPoint;
        private Transform _pitBottomPoint;
        private BoxCollider _pitBottomArea;
        private BoxCollider _pitTopArea;
        [Header("WallCrack")] 
        [SerializeField] private float timeToPassWallCrack;
        private bool _isWallCrackOnHover;
        private bool _isInWallCrackArea;
        private GameObject _wallCrack;
        private Transform _inStartPoint;
        private Transform _inTargetPoint;
        private Transform _outStartPoint;
        private Transform _outTargetPoint;
        [Header("InfoText")] 
        [SerializeField] private TMP_Text objInfoText;
        private string _objName;
        private bool _isCursorOnObj;
        
        private static readonly int IsCrouching = Animator.StringToHash("isCrouching");
        private static readonly int Stand = Animator.StringToHash("Stand");

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            Array.Fill(keysPicked,false);
            _characterController = GetComponent<CharacterController>();
            _cameraController = camHolder.GetComponent<CameraController>();
            _flashlightAndCameraController = GetComponent<FlashlightAndCameraController>();
            _headBobController = GetComponent<HeadBobController>();
            nrOfBatteries = 3;
            batteryText.text = nrOfBatteries + "/5";
            objInfoText.gameObject.SetActive(false);
            _inputActions = new GameInputActions();
            _inputActions.Player.Interact.performed += _ => Interact();
            StartCoroutine(RayCastCoroutine());
        }

        private void Start()
        {
            _pitBottomArea = GameObject.Find("PitBottomArea").GetComponent<BoxCollider>();
            _pitTopArea = GameObject.Find("PitTopArea").GetComponent<BoxCollider>();
            _inStartPoint = GameObject.Find("InStartPosition").GetComponent<Transform>();
            _inTargetPoint = GameObject.Find("InTargetPosition").GetComponent<Transform>();
            _outStartPoint = GameObject.Find("OutStartPosition").GetComponent<Transform>();
            _outTargetPoint = GameObject.Find("OutTargetPosition").GetComponent<Transform>();
        }

        private IEnumerator RayCastCoroutine()
        {
            WaitForSeconds wait = new WaitForSeconds(0.2f);
            while (true)
            {
                yield return wait;
                CursorRayCast();
                ObjInfoText();
            }
            // ReSharper disable once IteratorNeverReturns
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
                currentFrontOfTheLockerPoint = null;
                _currentExitPointLockerPoint = null;
                _isDoorPositionSet = false;
                
                _isSimpleDoorOnHover = false;
                _currentSimpleDoor = null;

                _isKeyOnHover = false;
                _key = null;
                
                _currentKeyDoor = null;
                _currentKeyDoorOpenPivot = null;
                _isKeyLockedDoorOnHover = false;

                _isFrontDoorOnHover = false;
                
                _isLadderOnHover = false;
                    
                _isCardReaderOnHover = false;

                _isRopeOnHover = false;

                _isWallCrackOnHover = false;
                
                _objName = "";
                _isCursorOnObj = false;
            }
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var rayCastHit, rayCastDistance))
            {
                if (rayCastHit.collider.gameObject.CompareTag("Battery"))
                {
                    _objName = "Battery";
                    _batteryOnHover = true;
                    _battery = rayCastHit.collider.gameObject;
                }

                if (rayCastHit.collider.gameObject.CompareTag("Corpse"))
                {
                    _corpseOnHover = true;
                    _currentCorpse = rayCastHit.collider.gameObject;
                }

                if (rayCastHit.collider.gameObject.CompareTag("LockerDoor"))
                {
                    _lockerDoorOnHover = true;
                    _objName = "Locker";
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
                        currentFrontOfTheLockerPoint = inFrontOfTheLockerTransform.gameObject;
                    }

                    Transform exitLockerPointTransform = _currentLocker.transform.Find("ExitLockerPoint");
                    if (exitLockerPointTransform != null)
                    {
                        _currentExitPointLockerPoint = exitLockerPointTransform.gameObject;
                    }

                    if (_currentDoorPivot != null && !_isDoorPositionSet)
                    {
                        _closedRotation = _currentDoorPivot.transform.rotation;
                        var eulerAngles = _currentDoorPivot.transform.eulerAngles;
                        _openRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y + 90f, eulerAngles.z);
                        _isDoorPositionSet = true;
                    }
                }

                if (!_isDoorOpening)
                {
                    if (rayCastHit.collider.gameObject.CompareTag("SimpleDoor"))
                    {
                        _isSimpleDoorOnHover = true;
                        _currentSimpleDoor = rayCastHit.collider.gameObject;
                        _currentDoorPivot = _currentSimpleDoor.transform.parent.gameObject;
                        GameObject simpleDoor = _currentDoorPivot.transform.parent.gameObject;
                        Transform currentSimpleDoorOpenPivotTransform = simpleDoor.transform.Find("openDoor");
                        if (currentSimpleDoorOpenPivotTransform != null)
                        {
                            _currentSimpleDoorOpenPivot = currentSimpleDoorOpenPivotTransform.gameObject;
                        }

                        Transform currentSimpleDoorClosePivotTransform = simpleDoor.transform.Find("closedDoor");
                        if (currentSimpleDoorClosePivotTransform != null)
                        {
                            _currentSimpleDoorClosedPivot = currentSimpleDoorClosePivotTransform.gameObject;
                        }

                        _simpleDoorOpenRotation = _currentSimpleDoorOpenPivot.transform.rotation;
                        _simpleDoorCloseRotation = _currentSimpleDoorClosedPivot.transform.rotation;
                        if (_currentDoorPivot.transform.rotation == _simpleDoorOpenRotation)
                        {
                            _isSimpleDoorOpen = true;
                        }
                        else if (_currentDoorPivot.transform.rotation == _simpleDoorCloseRotation)
                        {
                            _isSimpleDoorOpen = false;
                        }
                    }
                }
                else {_isSimpleDoorOnHover = false;}

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

                    if (_currentKeyDoorOpenPivot != null)
                        _keyDoorOpenRotation = _currentKeyDoorOpenPivot.transform.rotation;
                    _doorIdComponent = keyDoor.GetComponentInChildren<TMP_Text>();
                    _doorIdText = _doorIdComponent.text;
                    if (int.TryParse(_doorIdText, out int doorInt))
                    {
                        _doorIdInt = doorInt;
                    }
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
                    _objName = $"Key{_idInt}";
                }

                if (rayCastHit.collider.gameObject.CompareTag("FrontDoor"))
                {
                    _isFrontDoorOnHover = true;
                }

                if (rayCastHit.collider.gameObject.CompareTag("Ladder"))
                {
                    _objName = "Ladder";
                    _isLadderOnHover = true;
                    _ladder = rayCastHit.collider.gameObject;
                    _bottomPoint = GameObject.Find("BottomPoint").GetComponent<Transform>();
                    _upPoint = GameObject.Find("UpPoint").GetComponent<Transform>();
                }

                if (rayCastHit.collider.gameObject.CompareTag("KeyCard"))
                {
                    _objName = "Key Card";
                    _isKeyCardOnHover = true;
                    _keyCard = rayCastHit.collider.gameObject;
                }
                
                if (rayCastHit.collider.gameObject.CompareTag("CardReader"))
                {
                    _objName = "Card Reader";
                    _isCardReaderOnHover = true;
                    GameObject doubleDoorCard = rayCastHit.collider.gameObject.transform.parent.gameObject;
                    GameObject doubleDoors = doubleDoorCard.transform.Find("DoubleDoors").gameObject;
                    _leftCardDoor = doubleDoors.transform.Find("LeftDoor").gameObject;
                    _rightCardDoor = doubleDoors.transform.Find("RightDoor").gameObject;
                    _leftCardDoorOpenPoint = doubleDoors.transform.Find("LeftDoorOpenPoint");
                    _rightCardDoorOpenPoint = doubleDoors.transform.Find("RightDoorOpenPoint");
                }

                if (rayCastHit.collider.gameObject.CompareTag("Rope"))
                {
                    if (_characterController.bounds.Intersects(_pitBottomArea.bounds))
                    {
                        _isInPit = true;
                    }else if (_characterController.bounds.Intersects(_pitTopArea.bounds))
                    {
                        _isInPit = false;
                    }
                    _isRopeOnHover = true;
                    _objName = "Rope";
                    _rope = rayCastHit.collider.gameObject;
                    _pitBottomPoint = _rope.transform.Find("PitBottomPoint");
                    _pitTopPoint = _rope.transform.Find("PitTopPoint");
                }

                if (rayCastHit.collider.gameObject.CompareTag("WallCrack"))
                {
                    _isWallCrackOnHover = true;
                    _objName = "Wall Crack";
                    _wallCrack = rayCastHit.collider.gameObject;
                }

                _isCursorOnObj = true;
            }
            else
            {
                _objName = "";
                _isCursorOnObj = false;
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
                EnterHospital();
                ClimbGetDownLadder();
                PickKeyCard();
                StartCoroutine(ReadCardReader());
                ClimbGetDownRope();
                StartCoroutine(PassWallCrack());
            }
        }


        private void ObjInfoText()
        {
            if (_isCursorOnObj)
            {
                objInfoText.text = _objName;
                objInfoText.gameObject.SetActive(true);
            }
            else
            {
                objInfoText.gameObject.SetActive(false);
            }
        }
        
        
        private IEnumerator PassWallCrack()
        {
            if (_isWallCrackOnHover)
            {
                isInteracting = true;
                if (FirstPersonController.Instance.isCrouching)
                {
                    FirstPersonController.Instance.CrouchPressed();
                    AnimationController.Instance.animator.SetTrigger(Stand);
                    AnimationController.Instance.animator.SetBool(IsCrouching, false);
                    yield return StartCoroutine(FirstPersonController.Instance.CrouchStand());
                }
                _characterController.enabled = false;
                _cameraController.enabled = false;
                _headBobController.enabled = false;
                hands.SetActive(false);
                yield return StartCoroutine(TranslateThroughWallCrack());
                hands.SetActive(true);
                _characterController.enabled = true;
                _cameraController.enabled = true;
                _headBobController.enabled = true;
                isInteracting = false;
            }
        }

        private IEnumerator TranslateThroughWallCrack()
        {
            Vector3 startPosition = _isInWallCrackArea ? _outStartPoint.position : _inStartPoint.position;
            Vector3 targetPosition = _isInWallCrackArea ? _outTargetPoint.position : _inTargetPoint.position;
            Quaternion targetRotation = _isInWallCrackArea ? _outTargetPoint.rotation : _inTargetPoint.rotation;
            float timeElapsed = 0f;
            
            while (timeElapsed < timeToPassWallCrack)
            {
                player.transform.rotation = targetRotation;
                camHolder.transform.rotation = targetRotation;
                player.transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / timeToPassWallCrack);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            _isInWallCrackArea = !_isInWallCrackArea;
            player.transform.position = targetPosition;
            player.transform.rotation = targetRotation;
            camHolder.transform.rotation = targetRotation;
        }
        
        private void ClimbGetDownRope()
        {
            if (_isRopeOnHover)
            {
                _characterController.enabled = false;
                Vector3 worldTransform = _isInPit
                    ? _rope.transform.TransformPoint(_pitTopPoint.localPosition)
                    : _rope.transform.TransformPoint(_pitBottomPoint.localPosition);
                player.transform.position = worldTransform;
                _isInPit = !_isInPit;
                _characterController.enabled = true;
            }
        }
        
        private void PickKeyCard()
        {
            if (_isKeyCardOnHover)
            {
                isKeyCardPicked = true;
                Destroy(_keyCard);
            }
        }
        
        private IEnumerator ReadCardReader()
        {
            if (_isCardReaderOnHover)
            {
                if (isKeyCardPicked)
                {
                    isInteracting = true;
                    yield return accessText.text = "Access Granted";
                    StartCoroutine(FadeText(accessText));
                    yield return StartCoroutine(OpenCardDoors());
                    isInteracting = false;
                }
                else
                {
                    yield return accessText.text = "Access Denied";
                    yield return StartCoroutine(FadeText(accessText));
                }
            }
        }

        private IEnumerator OpenCardDoors()
        {
            Vector3 leftCardDoorStartPosition = _leftCardDoor.transform.position;
            Vector3 leftCardDoorTargetPosition = _leftCardDoorOpenPoint.transform.position;
            Vector3 rightCardDoorStartPosition = _rightCardDoor.transform.position;
            Vector3 rightCardDoorTargetPosition = _rightCardDoorOpenPoint.transform.position;
            float timeElapsed = 0f;

            while (timeElapsed < cardDoorOpeningDuration)
            {
                _leftCardDoor.transform.position = Vector3.Lerp(leftCardDoorStartPosition, leftCardDoorTargetPosition, timeElapsed / cardDoorOpeningDuration);
                _rightCardDoor.transform.position = Vector3.Lerp(rightCardDoorStartPosition, rightCardDoorTargetPosition, timeElapsed / cardDoorOpeningDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            _leftCardDoor.transform.position = leftCardDoorTargetPosition;
            _rightCardDoor.transform.position = rightCardDoorTargetPosition;
        }
        
        
        private void ClimbGetDownLadder()
        {
            if (_isLadderOnHover)
            {
                _characterController.enabled = false;
                Vector3 worldTransform = _isOnUpperFloor
                    ? _ladder.transform.TransformPoint(_bottomPoint.localPosition)
                    : _ladder.transform.TransformPoint(_upPoint.localPosition);
                player.transform.position = worldTransform;
                _isOnUpperFloor = !_isOnUpperFloor;
                _characterController.enabled = true;
            }
        }
        
        
        private void EnterHospital()
        {
            if (_isFrontDoorOnHover)
            {
                if (keysPicked[0])
                {
                    SceneManager.LoadScene(mainGameSceneId);
                }
                else
                {
                    StartCoroutine(FadeText(lockedText));
                }
            }
        }
        
        private IEnumerator OpenDoorWithKey()
        {
            if (_isKeyLockedDoorOnHover)
            {
                //Debug.Log($"Door status: {keysPicked[_doorIdInt]}");
                if (keysPicked[_doorIdInt])
                {
                    isInteracting = true;
                    lockedText.text = $"Key{_idInt} was used";
                    StartCoroutine(FadeText(lockedText));
                    yield return StartCoroutine(OpenKeyDoor());
                    isInteracting = false;
                    
                }
                else
                {
                    lockedText.text = $"Locked\nKey{_doorIdInt} is needed";
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
                _isDoorOpening = true;
                yield return StartCoroutine(ToggleSimpleDoor());
                _isDoorOpening = false;
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
                FirstPersonController.Instance.inputActions.Disable();
                if (FirstPersonController.Instance.isCrouching)
                {
                    FirstPersonController.Instance.CrouchPressed();
                    AnimationController.Instance.animator.SetTrigger(Stand);
                    AnimationController.Instance.animator.SetBool(IsCrouching, false);
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
                FirstPersonController.Instance.inputActions.Enable();
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
            if (_flashlightAndCameraController.isCameraOn)
            {
                if (_corpseOnHover && !IsCorpseAlreadyFound(_currentCorpse))
                {
                    AddGameObject(_currentCorpse);
                    _nrOfCorpses++;
                    corpsesFoundText.text = _nrOfCorpses + "/5";
                    StartCoroutine(FadeText(corpsesFoundText));
                }
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
