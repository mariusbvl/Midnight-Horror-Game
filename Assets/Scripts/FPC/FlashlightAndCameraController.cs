using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;
namespace FPC
{
    public class FlashlightAndCameraController : MonoBehaviour
    {
        public static FlashlightAndCameraController Instance {get; private set; }
        private GameInputActions _inputActions;

        [Header("Flashlight")] 
        [SerializeField] private GameObject handFlashlight;
        [SerializeField] private GameObject modelFlashlight;
        [SerializeField] private GameObject flashlight;
        [SerializeField] private Slider consumeSlider;
        private bool _isFlashlightOn;
        private bool _canConsumeBattery;
        private bool _isConsumingBattery;
        private Coroutine _batteryCoroutine;
        [Header("Camera")] 
        [SerializeField] private GameObject handCamera;
        [SerializeField] private GameObject modelCamera;
        [SerializeField] public GameObject recordingImage;
        private bool _isCameraOn;
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            _inputActions = new GameInputActions();
            consumeSlider.value = 100f;
            _isFlashlightOn = false;
            _isCameraOn = false;
            _inputActions.Player.AltInteract.performed += _ => ChangeFlashlightBool();
            _inputActions.Player.AltInteract.performed += _ => ChangeCameraBool();
            _inputActions.Player.ChangeItem.performed += _ => ScrollCameraAndFlashlight();
        }

       
        void Update()
        {
            SwitchFlashlight();
            SwitchCamera();
            ConsumeBattery();
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
                    _canConsumeBattery = true;
                }
                else
                {
                    _canConsumeBattery = false;
                }
               
            }
            else
            {
                _canConsumeBattery = true;
            }
        }
        private void ConsumeBattery()
        {
            CheckIfHasBattery();
            if (flashlight.activeSelf && _canConsumeBattery)
            {
                if (!_isConsumingBattery)
                {
                    _batteryCoroutine = StartCoroutine(ConsumeBatteryCoroutine());
                    _isConsumingBattery = true;
                }
            }
            else
            {
                if (_isConsumingBattery)
                {
                    StopCoroutine(_batteryCoroutine);
                    _isConsumingBattery = false;
                }
            }
        }

        private IEnumerator ConsumeBatteryCoroutine()
        {
            while (true)
            {
                consumeSlider.value -= 25f;
                yield return new WaitForSeconds(2f);
            }
        }
         private void ScrollCameraAndFlashlight()
        {
            _isFlashlightOn = false;
            _isCameraOn = false;
            flashlight.SetActive(false);
            recordingImage.SetActive(false);
            if (modelCamera.activeSelf && !modelFlashlight.activeSelf)
            {
                modelCamera.SetActive(false);
                handCamera.SetActive(false);
                modelFlashlight.SetActive(true);
                handFlashlight.SetActive(true);
            }else if (modelFlashlight.activeSelf && !modelCamera.activeSelf)
            {
                modelCamera.SetActive(true);
                handCamera.SetActive(true);
                modelFlashlight.SetActive(false);
                handFlashlight.SetActive(false);
            }
        }
        
        private void SwitchFlashlight()
        {
            if (_canConsumeBattery)
            {
                flashlight.SetActive(_isFlashlightOn);
            }
            else
            {
                flashlight.SetActive(false);
            }
        }

        private void ChangeFlashlightBool()
        {
            if (handFlashlight.activeSelf && modelFlashlight.activeSelf)
            {
                _isFlashlightOn = !_isFlashlightOn;
            }
        }

        private void SwitchCamera()
        {
            if (modelCamera.activeSelf)
            {
                handCamera.SetActive(!_isCameraOn);
                recordingImage.SetActive(_isCameraOn);
            }
        }
        private void ChangeCameraBool()
        {
            if (modelCamera.activeSelf)
            {
                _isCameraOn = !_isCameraOn;
            }
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
