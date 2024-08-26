using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;


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
        [HideInInspector] public int nrOfBatteries;
        [Header("Corpse")] 
        [SerializeField]public TMP_Text corpsesFoundText;
        private GameObject _currentCorpse;
        public GameObject[] foundCorpses;
        public int nrOfCorpses;
        private bool _corpseOnHover;
        public float fadeDuration = 1.0f;

        [Header("HideInLocker")] 
        [SerializeField] private GameObject playerMesh;
        [SerializeField] public GameObject player;
        [SerializeField] private GameObject hands;
        [SerializeField] private GameObject camHolder;
        [HideInInspector]public CameraController cameraController;
        [HideInInspector] public CharacterController characterController;
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
        [SerializeField] private GameObject[] keysGameObjects;
        public bool[] keysPicked;
        private bool _isKeyOnHover;
        private TMP_Text _idTextComponent;
        private string _idText;
        private int _idInt;
        public bool aKeyWasPicked;
        public bool canAddKey;
        [HideInInspector] public GameObject lastPickedKey;
        private GameObject _key;
        [Header("Front Door")] 
        private bool _isFrontDoorOnHover;
        [Header("Ladder")] 
        [SerializeField] private GameObject fadeInAnimation;
        [SerializeField] private GameObject fadeOutAnimation;
        private bool _isLadderOnHover;
        private bool _isOnUpperFloor;
        private GameObject _ladder;
        private Transform _bottomPoint;
        private Transform _upPoint;
        [Header("KeyCard")] 
        [SerializeField]public bool isKeyCardPicked;
        [SerializeField] private GameObject cardReader;
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
        [SerializeField] private GameObject ropeParent;
        private bool _isRopeOnHover;
        private bool _isInPit;
        private GameObject _rope;
        private Transform _pitTopPoint;
        private Transform _pitBottomPoint;
        [SerializeField]public BoxCollider pitBottomArea;
        [SerializeField] private BoxCollider pitTopArea;
        [Header("RopePile")] 
        [SerializeField] private GameObject ropePileImage;
        private bool _isRopePileOnHover;
        private GameObject _ropePile;
        private bool _isRopePilePicked;
        [Header("ThrowArea")]
        private bool _isThrowAreaOnHover;
        [SerializeField]private GameObject throwArea;
        private bool _isRopeThrown;
        [Header("WallCrack")] 
        [SerializeField] private float timeToPassWallCrack;
        private bool _isWallCrackOnHover;
        private bool _isInWallCrackArea;
        private Transform _inStartPoint;
        private Transform _inTargetPoint;
        private Transform _outStartPoint;
        private Transform _outTargetPoint;
        [Header("Phone")]
        private bool _isPhoneOnHover;
        [HideInInspector]public bool canCallPolice;
        [HideInInspector] public bool wasPoliceCalled;
        private GameObject _phoneObj;
        [Header("ExitDoor")] 
        private bool _isExitDoorOnHover;

        [Header("Safe")]
        [SerializeField] public GameObject puzzleCanvasBlur;
        [SerializeField] public GameObject safeCodeImage;
        [SerializeField] private Button safeButtonToSelectOnStart;
        [HideInInspector]public bool isSafeCodeActive;
        private bool _isSafeOnHover;
        [HideInInspector]public bool isSafeOpen;

        [Header("Book")] 
        public GameObject book;
        public GameObject bookPivot;
        public GameObject bookParent;
        public GameObject bookInitialPivot;
        public GameObject bookTargetPivot;
        private bool _isBookOnHover;
        private bool _isBookRotating;

        [Header("ElectricBox")] 
        [SerializeField] private Transform electricBoxPuzzlePosition;
        [SerializeField] public GameObject electricBoxPanel;
        [SerializeField] public GameObject electricTimerText;
        private bool _isElectricBoxOnHover;
        [HideInInspector] public bool isElectricBoxActive;
        [SerializeField] private Button switchButton;
        [SerializeField] private Button redButton;
        [HideInInspector] public bool isLittleElectricBoxOpen;

        [Header("FuelCanister")] 
        [SerializeField] private float throwForce;
        [SerializeField] private float torqueAmount;
        [SerializeField] private GameObject handForCanister;
        [SerializeField] private GameObject canister;
        [SerializeField] private Transform canisterInHandTransform;
        [SerializeField] private GameObject generatorPuzzleGameObject;
        [SerializeField] private Transform canisterInWorldTransform;
        [SerializeField] private LayerMask collisionLayers;
        private bool _isFuelCanisterOnHover;
        private bool _isCanisterInHand;
        private bool _hasCanisterHitObject;

        [Header("Generator")] 
        [SerializeField] private Slider generatorFillSlider;
        [SerializeField] private float timeToFill;
        [SerializeField] private GameObject generatorCameraLight;
        [SerializeField] private TMP_Text generatorText;
        private GameObject _generator;
        private bool _isGeneratorOnHover;
        private bool _isFillingGenerator;
        private bool _isGeneratorFilled;

        [Header("Lever Switch")]
        [SerializeField] private TMP_Text leverText;
        [SerializeField] private Transform handlePivot;
        [SerializeField] private Transform handleDraggedPivot;
        [SerializeField] private Transform switchDoorPivot;
        [SerializeField] private Transform switchDoorOpenPivot;
        [SerializeField] private float handleDragTime;
        [SerializeField] private float switchDoorOpenTime;
        private bool _isSwitchHandleOnHover;


        [Header("Electric Lever")] 
        private bool _isElectricLeverOnHover;
        [HideInInspector]public bool isLeverRotating;
        [HideInInspector] public GameObject electricLever;
        
        
        [Header("InfoText")] 
        [SerializeField] private TMP_Text objInfoText;
        private string _objName;
        private bool _isCursorOnObj;
        
        private static readonly int IsCrouching = Animator.StringToHash("isCrouching");
        private static readonly int Stand = Animator.StringToHash("Stand");

        [Header("AudioClips")] 
        [SerializeField] private AudioClip doorOpeningSound;
        [SerializeField] private AudioClip doorClosingSound;
        [SerializeField] private AudioClip cameraShotSound;
        [SerializeField] private AudioClip pickBatteryAndKeyCardSound;
        [SerializeField] private AudioClip pickKeySound;
        [SerializeField] private AudioClip lockerDoorOpenSound;
        [SerializeField] private AudioClip lockerDoorCloseSound;
        [SerializeField] private AudioClip keyRotateSound;
        [SerializeField] private AudioClip doorCreakSound;
        [SerializeField] private AudioClip doorLockedSound;
        [SerializeField] private AudioClip climbLadderAudio;
        [SerializeField] private AudioClip accessDeniedSound;
        [SerializeField] private AudioClip accessGrantedSound;
        [SerializeField] private AudioClip doubleDoorOpenSound;
        [SerializeField] private AudioClip climbRopeAudio;
        [SerializeField] private AudioClip wallCrackSound;
        [SerializeField] private AudioClip phoneRingingSound;
        [SerializeField] private AudioClip grabCanisterSound;
        [SerializeField] private AudioClip dropCanisterSound;
        [SerializeField] private AudioClip pouringFuelSound;
        private AudioSource _pouringFuelAudioSource;
        [SerializeField] private AudioClip noFuelGeneratorSound;
        [SerializeField] private AudioClip generatorSound;
        [SerializeField] private AudioClip dragLeverSound;
        [SerializeField] private AudioClip leverDoorSound;
        [SerializeField] private AudioClip leverHandleDeniedSound;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            Array.Fill(keysPicked,false);
            characterController = GetComponent<CharacterController>();
            cameraController = camHolder.GetComponent<CameraController>();
            _flashlightAndCameraController = GetComponent<FlashlightAndCameraController>();
            _headBobController = GetComponent<HeadBobController>();
            nrOfBatteries = 1;
            batteryText.text = nrOfBatteries + "/5";
            corpsesFoundText.text = "0/5";
            objInfoText.gameObject.SetActive(false);
            _inputActions = new GameInputActions();
            
            _inputActions.Player.Interact.performed += _ => Interact();
            _inputActions.Player.Interact.canceled += _ => ResetInteraction();
            _inputActions.Player.Throw.performed += _ => Throw();
            StartCoroutine(RayCastCoroutine());
        }

        private void Start()
        {
            try
            {
                _inStartPoint = GameObject.Find("InStartPosition").GetComponent<Transform>();
                _inTargetPoint = GameObject.Find("InTargetPosition").GetComponent<Transform>();
                _outStartPoint = GameObject.Find("OutStartPosition").GetComponent<Transform>();
                _outTargetPoint = GameObject.Find("OutTargetPosition").GetComponent<Transform>();
            }
            catch (NullReferenceException)
            {
                Debug.Log("No Pit and WallCrack in current scene");
            }
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
                lastPickedKey = null;
                aKeyWasPicked = false;
                canAddKey = false;
                _keyCard = null;
                _isKeyCardOnHover = false;
                
                _currentKeyDoor = null;
                _currentKeyDoorOpenPivot = null;
                _isKeyLockedDoorOnHover = false;

                _isFrontDoorOnHover = false;
                _isExitDoorOnHover = false;
                
                _isLadderOnHover = false;
                    
                _isCardReaderOnHover = false;

                _isRopeOnHover = false;
                _isThrowAreaOnHover = false;
                _isRopePileOnHover = false;
                
                _isWallCrackOnHover = false;
                
                _objName = "";
                _isCursorOnObj = false;

                _isPhoneOnHover = false;

                _isSafeOnHover = false;

                _isBookOnHover = false;
                book = null;
                bookParent = null;
                bookPivot = null;
                bookInitialPivot = null;
                bookTargetPivot = null;

                _isElectricBoxOnHover = false;

                _isFuelCanisterOnHover = false;

                _isGeneratorOnHover = false;

                _isSwitchHandleOnHover = false;

                _isElectricLeverOnHover = false;
                electricLever = null;
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
                    _objName = _key.name;
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
                    var objHit = rayCastHit.collider.gameObject;
                    cardReader = objHit;
                    GameObject doubleDoorCard = objHit.transform.parent.gameObject;
                    GameObject doubleDoors = doubleDoorCard.transform.Find("DoubleDoors").gameObject;
                    _leftCardDoor = doubleDoors.transform.Find("LeftDoor").gameObject;
                    _rightCardDoor = doubleDoors.transform.Find("RightDoor").gameObject;
                    _leftCardDoorOpenPoint = doubleDoors.transform.Find("LeftDoorOpenPoint");
                    _rightCardDoorOpenPoint = doubleDoors.transform.Find("RightDoorOpenPoint");
                }

                if (rayCastHit.collider.gameObject.CompareTag("Rope"))
                {
                    if (characterController.bounds.Intersects(pitBottomArea.bounds))
                    {
                        _isInPit = true;
                    }else if (characterController.bounds.Intersects(pitTopArea.bounds))
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
                }

                if (rayCastHit.collider.gameObject.CompareTag("Phone"))
                {
                    _objName = "Phone";
                    _isPhoneOnHover = true;
                    _phoneObj = rayCastHit.collider.gameObject;
                }

                if (rayCastHit.collider.gameObject.CompareTag("ExitDoor"))
                {
                    _isExitDoorOnHover = true;
                }
                
                if (rayCastHit.collider.gameObject.CompareTag("Safe"))
                {
                    _isSafeOnHover = true;
                }

                if (!_isBookRotating)
                {
                    if (rayCastHit.collider.gameObject.CompareTag("Book"))
                    {
                        _isBookOnHover = true;
                        var objectHit = rayCastHit.collider.gameObject;
                        book = objectHit;
                        bookPivot = objectHit.transform.parent.gameObject;
                        bookParent = bookPivot.transform.parent.gameObject;
                        bookInitialPivot = bookParent.transform.Find("initialPivot").gameObject;
                        bookTargetPivot = bookParent.transform.Find("targetPivot").gameObject;
                        _objName = bookParent.name;
                    }
                }
                else { _isBookOnHover = false;}
                
                if (rayCastHit.collider.gameObject.CompareTag("ElectricBox"))
                {
                    _isElectricBoxOnHover = true;
                    _objName = "Electricity Box";
                }
                
                if (rayCastHit.collider.gameObject.CompareTag("FuelCanister"))
                {
                    _isFuelCanisterOnHover = true;
                    _objName = "Fuel Canister";
                }
                if (rayCastHit.collider.gameObject.CompareTag("Generator"))
                {
                    _isGeneratorOnHover = true;
                    _isFillingGenerator = true;
                    _generator = rayCastHit.collider.gameObject;
                    _objName = "Generator";
                }
                
                if (rayCastHit.collider.gameObject.CompareTag("LeverSwitch"))
                {
                    _isSwitchHandleOnHover = true;
                    _objName = "Lever Switch";
                }

                if (!isLeverRotating)
                {
                    if (rayCastHit.collider.gameObject.CompareTag("ElectricLever"))
                    {
                        _isElectricLeverOnHover = true;
                        electricLever = rayCastHit.collider.gameObject;
                        _objName = "Electric Lever";
                    }
                }

                if (rayCastHit.collider.gameObject.CompareTag("RopePile"))
                {
                    _isRopePileOnHover = true;
                    _ropePile = rayCastHit.collider.gameObject;
                    _objName = "Rope Pile";
                }

                if (rayCastHit.collider.gameObject.CompareTag("ThrowArea"))
                {
                    _isThrowAreaOnHover = true;
                    throwArea = rayCastHit.collider.gameObject;
                    _objName = "Throw Rope";
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
            if (isInteracting) return;
            InteractWithBattery();
            StartCoroutine(PhotoCorpse());
            StartCoroutine(HideInLocker());
            StartCoroutine(ExitLocker());
            StartCoroutine(OpenCloseSimpleDoor());
            PickKey();
            StartCoroutine(OpenDoorWithKey());
            EnterHospital();
            StartCoroutine(ClimbGetDownLadder());
            PickKeyCard();
            StartCoroutine(ReadCardReader());
            StartCoroutine(ClimbGetDownRope());
            StartCoroutine(PassWallCrack());
            CallPolice();
            ExitHospital();
            OpenSafeCodePicker();
            StartCoroutine(InteractWithBook());
            StartCoroutine(OpenElectricBoxPuzzle());
            GrabCanister();
            StartCoroutine(FillGenerator());
            StartCoroutine(OpenLeverDoor());
            StartCoroutine(SwitchElectricLever());
            PickRopePile();
            ThrowRope();
        }

        private void Throw()
        {
            if (isInteracting) return;
            ThrowCanister();
        }


        private void ResetInteraction()
        {
            if (_isFillingGenerator) { _isFillingGenerator = false; }
        }
        
        private void ThrowRope()
        {
            if(!_isThrowAreaOnHover) return;
            if (!_isRopePilePicked) return;
            SoundFXManager.Instance.PlaySoundFxClip(pickBatteryAndKeyCardSound, player.transform, 1f, 0f);
            ropeParent.SetActive(true);
            ropePileImage.SetActive(false);
            Destroy(throwArea);
        }
        
        private void PickRopePile()
        {
            if(!_isRopePileOnHover) return;
            SoundFXManager.Instance.PlaySoundFxClip(pickBatteryAndKeyCardSound, player.transform, 1f, 0f);
            _isRopePilePicked = true;
            throwArea.SetActive(true);
            ropePileImage.SetActive(true);
            Destroy(_ropePile);
        }
        
        private IEnumerator SwitchElectricLever()
        {
            if(!_isElectricLeverOnHover) yield break;
            isInteracting = true;
            isLeverRotating = true;
            yield return StartCoroutine(LeverPuzzle.Instance.SwitchLever());
            isLeverRotating = false;
            isInteracting = false;
        }
        
        private IEnumerator OpenLeverDoor()
        {
            if(!_isSwitchHandleOnHover) yield break;
            if (_isGeneratorFilled)
            {
                SoundFXManager.Instance.PlaySoundFxClip(dragLeverSound, handlePivot, 1f, 1f);
                yield return StartCoroutine(DragLever());
                SoundFXManager.Instance.PlaySoundFxClip(leverDoorSound, switchDoorPivot, 1f,1f);
                yield return StartCoroutine((MoveLeverDoor()));
            }
            else
            {
                SoundFXManager.Instance.PlaySoundFxClip(leverHandleDeniedSound, handlePivot, 1f,1f);
                StartCoroutine(FadeText(leverText));
            }
        }

        private IEnumerator DragLever()
        {
            Quaternion startRotation = handlePivot.rotation;
            Quaternion targetRotation = handleDraggedPivot.rotation;
            float timeElapsed = 0;
            while (timeElapsed < handleDragTime)
            {
                handlePivot.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / handleDragTime);
                timeElapsed += Time.deltaTime; 
                yield return null;
            }
            handlePivot.rotation = targetRotation;
            yield return null;
        }

        private IEnumerator MoveLeverDoor()
        {
            Vector3 startPosition = switchDoorPivot.position;
            Vector3 targetPosition = switchDoorOpenPivot.position;

            float timeElapsed = 0;
            while (timeElapsed < switchDoorOpenTime)
            {
                switchDoorPivot.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / switchDoorOpenTime);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            switchDoorPivot.position = targetPosition;
        }
        private IEnumerator FillGenerator()
        {
            if (_isGeneratorOnHover && !_isCanisterInHand && !_isGeneratorFilled)
            {
                SoundFXManager.Instance.PlaySoundFxClip(noFuelGeneratorSound, _generator.transform, 1f,1f);
                StartCoroutine(FadeText(generatorText));
                yield break;
            }

            if (!_isGeneratorOnHover || !_isCanisterInHand || _isGeneratorFilled) yield break;
            
            generatorFillSlider.gameObject.SetActive(true);
            float elapsedTime = 0f; 
            generatorFillSlider.value = 0f; 
            SoundFXManager.Instance.PlaySoundFxClip(pouringFuelSound, player.transform, 1f,0f,true, timeToFill);
            while (elapsedTime < timeToFill)
            {
                if (CancelFilling()) yield break;

                elapsedTime += Time.deltaTime;
                generatorFillSlider.value = Mathf.Lerp(0f, 100f, elapsedTime / timeToFill);
                yield return null;
            }

            generatorFillSlider.value = 100f;
            generatorFillSlider.gameObject.SetActive(false);
            _isGeneratorFilled = true;
            generatorCameraLight.SetActive(true);
            SoundFXManager.Instance.PlaySoundFxClip(generatorSound, _generator.transform, 0.3f,1f, true, 3600);
        }

        private bool CancelFilling()
        {
            if (_isGeneratorOnHover && _isFillingGenerator && _isCanisterInHand) return false;
            _pouringFuelAudioSource = SoundFXManager.Instance.audioSource;
            Destroy(_pouringFuelAudioSource.gameObject);
            _pouringFuelAudioSource = null;
            generatorFillSlider.gameObject.SetActive(false);
            return true;
        }

        
        private void GrabCanister()
        {
            if(!_isFuelCanisterOnHover) return;
            SoundFXManager.Instance.PlaySoundFxClip(grabCanisterSound, player.transform, 1f, 0f);
            BoxCollider canisterCollider = canister.GetComponent<BoxCollider>();
            Rigidbody canisterRigidBody = canister.GetComponent<Rigidbody>();
            hands.SetActive(false);
            handForCanister.SetActive(true);
            canisterCollider.enabled = false;
            Destroy(canisterRigidBody);
            canister.transform.SetParent(canisterInHandTransform);
            canister.transform.localPosition = new Vector3(0f, 0f, 0f);
            canister.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            canister.transform.localScale = new Vector3(1f, 1f, 1f);
            _isCanisterInHand = true;
        }

        private void ThrowCanister()
        {
            if(!_isCanisterInHand) return;
            canister.transform.SetParent(generatorPuzzleGameObject.transform);
            canister.transform.localScale = canisterInWorldTransform.localScale;
            BoxCollider canisterCollider = canister.GetComponent<BoxCollider>();
            canisterCollider.enabled = true;
            Rigidbody canisterRigidBody = canister.AddComponent<Rigidbody>();
            Vector3 throwDirection = transform.forward;
            canisterRigidBody.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            canisterRigidBody.AddTorque(new Vector3(Random.Range(-torqueAmount, torqueAmount), Random.Range(-torqueAmount, torqueAmount), Random.Range(-torqueAmount, torqueAmount)), ForceMode.Impulse);
            _isCanisterInHand = false;
            handForCanister.SetActive(false);
            hands.SetActive(true);
            _hasCanisterHitObject = false;
            StartCoroutine(CheckCanisterCollision());
        }
        private IEnumerator CheckCanisterCollision()
        {
            BoxCollider canisterCollider = canister.GetComponent<BoxCollider>();
            while (!_hasCanisterHitObject)
            {
                var bounds = canisterCollider.bounds;
                Collider[] colliders = Physics.OverlapBox(bounds.center, bounds.extents, canister.transform.rotation, collisionLayers);
            
                foreach (Collider collider1 in colliders)
                {
                    if (collider1.gameObject != canister && collider1.gameObject != gameObject)
                    {
                        _hasCanisterHitObject = true;
                        break;
                    }
                }
                yield return null; 
            }
            SoundFXManager.Instance.PlaySoundFxClip(dropCanisterSound, canister.transform, 1f, 1f);
        }
        
        private IEnumerator OpenElectricBoxPuzzle()
        {
            if(!_isElectricBoxOnHover) yield break;
            if(isElectricBoxActive) yield break;
            if(isLittleElectricBoxOpen) yield break;
            ElectricBoxPuzzle.Instance.ResetPuzzle();
            FirstPersonController.Instance.inputActions.Disable();
            if (FirstPersonController.Instance.isCrouching)
            {
                FirstPersonController.Instance.CrouchPressed();
                AnimationController.Instance.animator.SetTrigger(Stand);
                AnimationController.Instance.animator.SetBool(IsCrouching, false);
                yield return StartCoroutine(FirstPersonController.Instance.CrouchStand());
            }
            characterController.enabled = false;
            cameraController.enabled = false;
            hands.SetActive(false);
            _flashlightAndCameraController.enabled = false;
            FlashlightAndCameraController.Instance.isFlashlightOn = false;
            FlashlightAndCameraController.Instance.isCameraOn = false;
            FlashlightAndCameraController.Instance.flashlight.SetActive(false);
            FlashlightAndCameraController.Instance.recordingImage.SetActive(false);
            FlashlightAndCameraController.Instance.ConsumeBattery();
            player.transform.position = electricBoxPuzzlePosition.position;
            var rotation = electricBoxPuzzlePosition.rotation;
            player.transform.rotation =  rotation;
            camHolder.transform.rotation = rotation;
            GameManager.Instance.playerCanvas.SetActive(false);
            electricBoxPanel.SetActive(true);
            electricTimerText.SetActive(true);
            isElectricBoxActive = true;
            Cursor.lockState = CursorLockMode.None;
            GameManager.Instance.inputActions.Player.Pause.performed -= GameManager.Instance.pausePerformedHandler;
            GameManager.Instance.inputActions.Player.Pause.performed += GameManager.Instance.closeElectricBoxHandler;
            redButton.gameObject.SetActive(false);
            GameObject button;
            (button = switchButton.gameObject).SetActive(true);
            EventSystem.current.SetSelectedGameObject(button);
            ElectricBoxPuzzle.Instance.timerText.text = $"{ElectricBoxPuzzle.Instance.puzzleSeconds}";
            Debug.Log("Electric Box clicked");
        }
        
        private IEnumerator InteractWithBook()
        {
            if (!_isBookOnHover) yield break;
            isInteracting = true;
            _isBookRotating = true;
            yield return StartCoroutine(BookShelfPuzzle.Instance.InteractWithBook());
            _isBookRotating = false;
            isInteracting = false;
        }
        
        
        
        
        private void OpenSafeCodePicker()
        {
            if(!_isSafeOnHover) return;
            if(isSafeOpen) return;
            characterController.enabled = false;
            cameraController.enabled = false;
            _flashlightAndCameraController.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            GameManager.Instance.playerCanvas.SetActive(false);
            hands.SetActive(false);
            puzzleCanvasBlur.SetActive(true);
            safeCodeImage.SetActive(true);
            isSafeCodeActive = true;
            GameManager.Instance.inputActions.Player.Pause.performed -= GameManager.Instance.pausePerformedHandler;
            GameManager.Instance.inputActions.Player.Pause.performed += GameManager.Instance.closeSafeCodePickerHandler;
            EventSystem.current.SetSelectedGameObject(safeButtonToSelectOnStart.gameObject);
        }
        
        private void ExitHospital()
        {
            if (!_isExitDoorOnHover) return;
            if (GameManager.Instance.canExitHospital)
            {
                SoundFXManager.Instance.PlaySoundFxClip(keyRotateSound, player.transform, 1f, 0f);
                SaveManager.Instance.SaveBatteries();
                _inputActions.Disable();
                GameManager.Instance.loadingManagerMainMenu.canvasToDisable = GameObject.Find("PlayerCanvas");
                GameManager.Instance.loadingManagerMainMenu.LoadScene(4);
            }else
            {
                SoundFXManager.Instance.PlaySoundFxClip(doorLockedSound, player.transform, 1f, 0f);
            }
        }
        private void EnterHospital()
        {
            if (_isFrontDoorOnHover)
            {
                if (keysPicked[0])
                {
                    SoundFXManager.Instance.PlaySoundFxClip(keyRotateSound, player.transform, 1f, 0f);
                    SaveManager.Instance.SaveBatteries();
                    _inputActions.Disable();
                    OutsideGameManager.Instance.loadingManagerMainMenu.canvasToDisable = GameObject.Find("PlayerCanvas").gameObject;
                    OutsideGameManager.Instance.loadingManagerMainMenu.LoadScene(3);
                }
                else
                {
                    SoundFXManager.Instance.PlaySoundFxClip(doorLockedSound, player.transform, 1f, 0f);
                    StartCoroutine(FadeText(lockedText));
                }
            }
        }
        private void CallPolice()
        {
            if (!_isPhoneOnHover) return;
            if (!canCallPolice || wasPoliceCalled) return;
            SoundFXManager.Instance.PlaySoundFxClip(phoneRingingSound, _phoneObj.transform, 1f, 1f, true, GameManager.Instance.callSeconds);
            GameManager.Instance.StartPoliceTimer();
            wasPoliceCalled = true;
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
                characterController.enabled = false;
                cameraController.enabled = false;
                _headBobController.enabled = false;
                hands.SetActive(false);
                SoundFXManager.Instance.PlaySoundFxClip(wallCrackSound, player.transform, 1f, 0f);
                yield return StartCoroutine(TranslateThroughWallCrack());
                hands.SetActive(true);
                characterController.enabled = true;
                cameraController.enabled = true;
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
        
        private void PickKey()
        {
            if (_isKeyOnHover)
            {
                SoundFXManager.Instance.PlaySoundFxClip(pickKeySound, player.transform, 1f, 0f);
                keysPicked[_idInt] = true;
                lastPickedKey = _key;
                _key.SetActive(false);
                aKeyWasPicked = true;
                canAddKey = true;
            }
        }
        private void PickKeyCard()
        {
            if (_isKeyCardOnHover)
            {
                SoundFXManager.Instance.PlaySoundFxClip(pickBatteryAndKeyCardSound, player.transform, 1f, 0f);
                isKeyCardPicked = true;
                lastPickedKey = _keyCard;
                _keyCard.SetActive(false);
                aKeyWasPicked = true;
                canAddKey = true;
            }
        }
        
        private IEnumerator ReadCardReader()
        {
            if (_isCardReaderOnHover)
            {
                if(_leftCardDoor.transform.position == _leftCardDoorOpenPoint.position) yield break;
                if (isKeyCardPicked)
                {
                    isInteracting = true;
                    SoundFXManager.Instance.PlaySoundFxClip(accessGrantedSound, cardReader.transform, 0.5f, 1f);
                    yield return accessText.text = "Access Granted";
                    StartCoroutine(FadeText(accessText));
                    SoundFXManager.Instance.PlaySoundFxClip(doubleDoorOpenSound, _leftCardDoor.transform, 1f, 1f);
                    yield return StartCoroutine(OpenCardDoors());
                    isInteracting = false;
                }
                else
                {
                    SoundFXManager.Instance.PlaySoundFxClip(accessDeniedSound, cardReader.transform, 0.5f, 1f);
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
        
        private IEnumerator ClimbGetDownRope()
        {
            if (!_isRopeOnHover) yield break;
            isInteracting = true;
            SoundFXManager.Instance.PlaySoundFxClip(climbRopeAudio, player.transform, 1f, 0f);
            characterController.enabled = false;
            _headBobController.enabled = false;
            fadeInAnimation.SetActive(true);
            yield return new WaitForSeconds(1f);
            Vector3 worldTransform = _isInPit
                ? _rope.transform.TransformPoint(_pitTopPoint.localPosition)
                : _rope.transform.TransformPoint(_pitBottomPoint.localPosition);
            player.transform.position = worldTransform;
            _isInPit = !_isInPit;
            characterController.enabled = true;
            _headBobController.enabled = true;
            fadeInAnimation.SetActive(false);
            fadeOutAnimation.SetActive(true);
            yield return new WaitForSeconds(1f);
            fadeOutAnimation.SetActive(false);
            isInteracting = false;
        }
        
        private IEnumerator ClimbGetDownLadder()
        {
            if (!_isLadderOnHover) yield break;
            isInteracting = true;
            SoundFXManager.Instance.PlaySoundFxClip(climbLadderAudio, player.transform, 1f, 0f);
            characterController.enabled = false;
            _headBobController.enabled = false;
            fadeInAnimation.SetActive(true);
            yield return new WaitForSeconds(1f);
            Vector3 worldTransform = _isOnUpperFloor
                ? _ladder.transform.TransformPoint(_bottomPoint.localPosition)
                : _ladder.transform.TransformPoint(_upPoint.localPosition);
            player.transform.position = worldTransform;
            _isOnUpperFloor = !_isOnUpperFloor;
            characterController.enabled = true;
            _headBobController.enabled = true;
            fadeInAnimation.SetActive(false);
            fadeOutAnimation.SetActive(true);
            yield return new WaitForSeconds(1f);
            fadeOutAnimation.SetActive(false);
            isInteracting = false;
        }
        
        
        private IEnumerator OpenDoorWithKey()
        {
            if (!_isKeyLockedDoorOnHover) yield break;
            if (_currentDoorPivot.transform.rotation == _currentKeyDoorOpenPivot.transform.rotation) yield break;
            //Debug.Log($"Door status: {keysPicked[_doorIdInt]}");
            if (keysPicked[_doorIdInt])
            {
                isInteracting = true;
                SoundFXManager.Instance.PlaySoundFxClip(keyRotateSound, _currentKeyDoor.transform, 1f, 1f);
                yield return new WaitForSeconds(0.5f);
                lockedText.text = $"{keysGameObjects[_doorIdInt].name} was used";
                StartCoroutine(FadeText(lockedText));
                SoundFXManager.Instance.PlaySoundFxClip(doorCreakSound, _currentKeyDoor.transform, 1f, 1f);
                yield return StartCoroutine(OpenKeyDoor());
                isInteracting = false;
                    
            }
            else
            {
                lockedText.text = $"Locked\n{keysGameObjects[_doorIdInt].name} is needed";
                SoundFXManager.Instance.PlaySoundFxClip(doorLockedSound, _currentKeyDoor.transform, 1f, 1f);
                yield return StartCoroutine(FadeText(lockedText));
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
            if (_lockerDoorOnHover && FirstPersonController.Instance.isHidden)
            {
                isInteracting= true;
                player.transform.position = _currentHideCameraPoint.transform.position;
                player.transform.rotation = _currentHideCameraPoint.transform.rotation;
                SoundFXManager.Instance.PlaySoundFxClip(lockerDoorOpenSound, _currentLockerDoor.transform, 1f, 1f);
                yield return StartCoroutine(ToggleDoor());
                yield return StartCoroutine(SmoothTransitionCoroutine());
                yield return characterController.enabled = true;
                yield return cameraController.enabled = true;
                _flashlightAndCameraController.enabled = true;
                _headBobController.enabled = true;
                playerMesh.SetActive(true);
                hands.SetActive(true);
                yield return StartCoroutine(ToggleDoor());
                SoundFXManager.Instance.PlaySoundFxClip(lockerDoorCloseSound, _currentLockerDoor.transform, 1f, 1f);
                yield return FirstPersonController.Instance.isHidden = false;
                yield return isInteracting = false;
            }
        }
        private IEnumerator HideInLocker()
        {
            if (_lockerDoorOnHover && !FirstPersonController.Instance.isHidden)
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
                characterController.enabled = false;
                cameraController.enabled = false;
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
                
                SoundFXManager.Instance.PlaySoundFxClip(lockerDoorOpenSound, _currentLockerDoor.transform, 1f, 1f);
                yield return StartCoroutine(ToggleDoor());

                yield return StartCoroutine(SmoothTransitionCoroutine());

                yield return StartCoroutine(ToggleDoor());
                SoundFXManager.Instance.PlaySoundFxClip(lockerDoorCloseSound, _currentLockerDoor.transform, 1f, 1f);
                yield return FirstPersonController.Instance.isHidden = true;
                yield return isInteracting= false;
                FirstPersonController.Instance.inputActions.Enable();
            }
        }

        private IEnumerator SmoothTransitionCoroutine()
        {
            float elapsedTime = 0f;
            Vector3 initialPosition = player.transform.position;
            Quaternion initialRotation = player.transform.rotation;
            Vector3 targetPosition = FirstPersonController.Instance.isHidden
                ? _currentExitPointLockerPoint.transform.position
                : _currentHideCameraPoint.transform.position;
            Quaternion targetRotation = FirstPersonController.Instance.isHidden
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
                SoundFXManager.Instance.PlaySoundFxClip(_isSimpleDoorOpen ? doorClosingSound : doorOpeningSound, _currentSimpleDoor.transform, 1f,1f);
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
            if (!_batteryOnHover) return;
            if(nrOfBatteries == 5) return;
            SoundFXManager.Instance.PlaySoundFxClip(pickBatteryAndKeyCardSound, player.transform, 1f, 0f);
            nrOfBatteries++;
            batteryText.text = nrOfBatteries + "/5";
            Destroy(_battery);
        }
        
        private IEnumerator PhotoCorpse()
        {
            if (!_flashlightAndCameraController.isCameraOn) yield break;
            SoundFXManager.Instance.PlaySoundFxClip(cameraShotSound, player.transform, 1f, 0f);
            if (_corpseOnHover && !IsCorpseAlreadyFound(_currentCorpse))
            {
                AddGameObject(_currentCorpse);
                if (_currentCorpse == GameManager.Instance.tunnelCorpse) { BookShelfPuzzle.Instance.bookshelfPivot.position = BookShelfPuzzle.Instance.downBookshelfPivot.position;}
                nrOfCorpses++;
                corpsesFoundText.text = nrOfCorpses + "/5";
                StartCoroutine(FadeText(corpsesFoundText));
                if (nrOfCorpses != 5) { GameManager.Instance.ChangeObjective();}
                GameManager.Instance.CheckForAllCorpses();
                yield return StartCoroutine(PlayFlashBackCoroutine(_currentCorpse));
            }
        }

        private IEnumerator PlayFlashBackCoroutine(GameObject currentCorpse)
        {
            float randomTime = Random.Range(1f, 3f);
            yield return new WaitForSeconds(randomTime);
            FlashbackManager.Instance.PlayFlashback(FoundCorpseId(currentCorpse));
        }
        
        private int FoundCorpseId(GameObject currentCorpse)
        {
            for (int i = 0; i < FlashbackManager.Instance.allCorpses.Length; i++)
            {
                if (FlashbackManager.Instance.allCorpses[i] == currentCorpse)
                {
                    return i;
                }
            }
            return -1;
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
        public IEnumerator FadeText(TMP_Text text)
        {
            if (text.gameObject.activeSelf) yield break;
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
            text.color = color;
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
