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
                }
            }
        }

        private void Interact()
        {
            InteractWithBattery();
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
