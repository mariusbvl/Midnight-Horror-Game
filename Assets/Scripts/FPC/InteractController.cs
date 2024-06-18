using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

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
        public GameObject[] _foundCorpses;
        private float _nrOfCorpses;
        private bool _corpseOnHover;
        public float fadeDuration = 1.0f;
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
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
                }
            }
        }

        private void Interact()
        {
            InteractWithBattery();
            PhotoCorpse();
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
            foreach (GameObject foundCorpse in _foundCorpses)
            {
                if (foundCorpse == corpse)
                {
                    return true;
                }
            }
            return false;
        }
        public void AddGameObject(GameObject obj)
        {
            GameObject[] newArray = new GameObject[_foundCorpses.Length + 1];
            for (int i = 0; i < _foundCorpses.Length; i++)
            {
                newArray[i] = _foundCorpses[i];
            }
            newArray[_foundCorpses.Length] = obj;
            _foundCorpses = newArray;
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
