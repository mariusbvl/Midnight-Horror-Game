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
        public GameObject currentLockerDoor;
        public GameObject currentLocker;
        public GameObject currentDoorPivot;
        public GameObject currentHideCameraPoint;
        public GameObject currentFrontOfTheLockerPoint;
        private bool _lockerDoorOnHover;
        //Locker Door Variables
        public float doorOpeningDuration = 0.5f;
        private Quaternion _closedRotation;
        private Quaternion _openRotation;
        private bool _isDoorPositionSet;
        private bool _isOpen;
        //Moving into locker
        private Coroutine transitionCoroutine;
        public float transitionDuration = 1.0f;
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            _characterController = GetComponent<CharacterController>();
            _cameraController = camHolder.GetComponent<CameraController>();
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
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var rayCastHit, rayCastDistance))
            {
                if (rayCastHit.collider.gameObject.CompareTag("Battery"))
                {
                    _batteryOnHover = true;
                    _battery = rayCastHit.collider.gameObject;
                    print("battery");
                }
                else
                {
                    _batteryOnHover = false;
                    _battery = null;
                }

                if (FlashlightAndCameraController.Instance.recordingImage.activeSelf)
                {
                    rayCastDistance = 3;
                    if (rayCastHit.collider.gameObject.CompareTag("Corpse"))
                    {
                        _corpseOnHover = true;
                        _currentCorpse = rayCastHit.collider.gameObject;
                        print("Corpse");
                    }
                    else
                    {
                        _corpseOnHover = false;
                        _currentCorpse = null;
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
                    if (currentDoorPivot != null && !_isDoorPositionSet)
                    {
                        _closedRotation = currentDoorPivot.transform.rotation;
                        var eulerAngles = currentDoorPivot.transform.eulerAngles;
                        _openRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y + 90f, eulerAngles.z);
                        _isDoorPositionSet = true;
                    }
                    print("LockerDoor");
                }
                else 
                {
                    _lockerDoorOnHover = false;
                    currentLockerDoor = null;
                    currentDoorPivot = null;
                    currentLocker = null;
                    currentHideCameraPoint = null;
                    _isDoorPositionSet = false;
                }
            }
        }

        private void Interact()
        {
            InteractWithBattery();
            PhotoCorpse();
            HideInLocker();
        }
        
        private void HideInLocker()
        {
            if (_lockerDoorOnHover)
            {
                ToggleDoor();
                _characterController.enabled = false;
                _cameraController.enabled = false;
                playerMesh.SetActive(false);
                hands.SetActive(false);
                player.transform.position = currentFrontOfTheLockerPoint.transform.position;
                player.transform.rotation = currentFrontOfTheLockerPoint.transform.rotation;
                transitionCoroutine = StartCoroutine(SmoothTransitionCoroutine());
                ToggleDoor();
            }
        }
        IEnumerator SmoothTransitionCoroutine()
        {
            float elapsedTime = 0f;

            Vector3 initialPosition = player.transform.position;
            Quaternion initialRotation = player.transform.rotation;

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / transitionDuration;

                // Smoothly move the position
                transform.position = Vector3.Lerp(initialPosition, currentHideCameraPoint.transform.position, t);

                // Smoothly rotate towards the target rotation
                transform.rotation = Quaternion.Slerp(initialRotation, currentHideCameraPoint.transform.rotation, t);

                yield return null; // Wait until the next frame
            }

            // Ensure the final position and rotation match exactly
            transform.position = currentHideCameraPoint.transform.position;
            transform.rotation = currentHideCameraPoint.transform.rotation;
        }
        private void ToggleDoor()
        {
            if (currentDoorPivot != null)
            {
                StartCoroutine(_isOpen ? RotateDoor(_closedRotation) : RotateDoor(_openRotation));
                _isOpen = !_isOpen;
                Debug.Log("Door state toggled. isOpen: " + _isOpen);
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
            Debug.Log(obj.name + " has been added to the array.");
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
