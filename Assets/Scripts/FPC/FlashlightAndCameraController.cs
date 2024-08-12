using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace FPC
{
    public class FlashlightAndCameraController : MonoBehaviour
    {
        public static FlashlightAndCameraController Instance {get; private set; }
        private GameInputActions _inputActions;

        [Header("Flashlight")] 
        [SerializeField] private GameObject handFlashlight;
        [SerializeField] private GameObject modelFlashlight;
        [SerializeField] public GameObject flashlight;
        [SerializeField] private Slider consumeSlider;
        [SerializeField] private float consumeValue;
        [SerializeField] private TMP_Text lowBatteryText;
        [SerializeField] private float fadeDuration;
        [SerializeField] private float blinkInterval;
        [HideInInspector]public bool isFlashlightOn;
        [HideInInspector]public bool canConsumeBattery;
        private bool _isLowBattery;
        private bool _isConsumingBattery;
        private Coroutine _batteryCoroutine;
        private Coroutine _blinkTextCoroutine;
        [Header("Camera")] 
        [SerializeField] private GameObject handCamera;
        [SerializeField] private GameObject modelCamera;
        [SerializeField] public GameObject recordingImage;
        [SerializeField] private GameObject batteryIcons;
        [SerializeField] private float defaultRayCastDistance;
        [SerializeField] private float cameraRayCastDistance;
        [HideInInspector]public bool isCameraOn;

        [Header("AudioClips")] 
        [SerializeField] private AudioClip flashlightOnSound;
        [SerializeField] private AudioClip flashlightOffSound;
        [SerializeField] private AudioClip cameraClickOpen;
        [SerializeField] private AudioClip cameraClickClose;
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            _inputActions = new GameInputActions();
            consumeSlider.value = 100f;
            isFlashlightOn = false;
            isCameraOn = false;
            _inputActions.Player.AltInteract.performed += _ => ChangeFlashlightBool();
            _inputActions.Player.AltInteract.performed += _ => ChangeCameraBool();
            _inputActions.Player.ChangeItem.performed += _ => ScrollCameraAndFlashlight();
        }

       
        void Update()
        {
            SwitchFlashlight();
            SwitchCamera();
            ConsumeBattery();
            RayCastDistanceController();
        }
        
        private void CheckIfHasBattery()
        {
            if (consumeSlider.value == 0)
            {
                if (InteractController.Instance.nrOfBatteries > 0)
                {
                    InteractController.Instance.nrOfBatteries--;
                    InteractController.Instance.batteryText.text = InteractController.Instance.nrOfBatteries + "/5";
                    consumeSlider.value = 100f; 
                    canConsumeBattery = true;
                }
                else
                {
                    canConsumeBattery = false;
                }
               
            }
            else
            {
                canConsumeBattery = true;
            }
        }
        public void ConsumeBattery()
        {
            CheckIfHasBattery();
            CheckIfLowBattery();
            if (flashlight.activeSelf && canConsumeBattery)
            {
                if (_isConsumingBattery) return;
                _batteryCoroutine = StartCoroutine(ConsumeBatteryCoroutine());
                _isConsumingBattery = true;
            }
            else
            {
                if (!_isConsumingBattery) return;
                StopCoroutine(_batteryCoroutine);
                _isConsumingBattery = false;
            }
        }

        private IEnumerator ConsumeBatteryCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                consumeSlider.value -= consumeValue;
                yield return new WaitForSeconds(1f);
            }
        }

        private void CheckIfLowBattery()
        {
            if (InteractController.Instance.nrOfBatteries == 0 && _isConsumingBattery && consumeSlider.value < 10f)
            {
                _isLowBattery = true;
            }
            else
            {
                _isLowBattery = false;
            }

            if (_isLowBattery)
            {
                _blinkTextCoroutine ??= StartCoroutine(BlinkText(lowBatteryText));
            }
            else
            {
                if (_blinkTextCoroutine == null) return;
                StopCoroutine(_blinkTextCoroutine);
                lowBatteryText.gameObject.SetActive(false);
                _blinkTextCoroutine = null;
            }
        }
         private void ScrollCameraAndFlashlight()
        {
            isFlashlightOn = false;
            isCameraOn = false;
            flashlight.SetActive(false);
            recordingImage.SetActive(false);
            if (modelCamera.activeSelf && !modelFlashlight.activeSelf)
            {
                modelCamera.SetActive(false);
                handCamera.SetActive(false);
                modelFlashlight.SetActive(true);
                handFlashlight.SetActive(true);
                batteryIcons.SetActive(true);
            }else if (modelFlashlight.activeSelf && !modelCamera.activeSelf)
            {
                modelCamera.SetActive(true);
                handCamera.SetActive(true);
                modelFlashlight.SetActive(false);
                handFlashlight.SetActive(false);
                batteryIcons.SetActive(true);
            }
        }
        
        private void SwitchFlashlight()
        {
            flashlight.SetActive(canConsumeBattery && isFlashlightOn);
        }

        private void ChangeFlashlightBool()
        {
            if (!handFlashlight.activeSelf || !modelFlashlight.activeSelf) return;
            SoundFXManager.Instance.PlaySoundFxClip(isFlashlightOn ? flashlightOnSound : flashlightOffSound,
                flashlight.transform, 1f, 0f);
            isFlashlightOn = !isFlashlightOn;
        }

        private void RayCastDistanceController()
        {
            InteractController.Instance.rayCastDistance = recordingImage.activeSelf ? cameraRayCastDistance : defaultRayCastDistance;
        }

        private void SwitchCamera()
        {
            if (modelCamera.activeSelf)
            {
                handCamera.SetActive(!isCameraOn);
                batteryIcons.SetActive(!isCameraOn);
                recordingImage.SetActive(isCameraOn);
            }
        }

        private void ChangeCameraBool()
        {
            if (modelCamera.activeSelf)
            {
                SoundFXManager.Instance.PlaySoundFxClip(isCameraOn ? cameraClickOpen : cameraClickClose,
                    modelCamera.transform, 1f, 0f);
                isCameraOn = !isCameraOn;
            }
        }

        public IEnumerator BlinkText(TMP_Text text)
        {
            if (text.gameObject.activeSelf) yield break;
            text.gameObject.SetActive(true);
            while (_isLowBattery)
            {   
                yield return StartCoroutine(Fade(0f, 1f, text));
                yield return new WaitForSeconds(blinkInterval);
                yield return StartCoroutine(Fade(1f, 0f, text));
            }
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
