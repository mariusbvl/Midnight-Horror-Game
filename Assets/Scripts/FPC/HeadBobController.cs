using UnityEngine;

namespace FPC
{
    public class HeadBobController : MonoBehaviour
    {
        [SerializeField] public bool enableBob = true;

        [SerializeField, Range(0, 0.1f)] public float amplitude;
        [SerializeField, Range(0, 30)] public float frequency;

        [SerializeField] private new Transform camera;
        [SerializeField] private Transform cameraHolder;
        [SerializeField] private Transform hands;
        
        private readonly float _toggleSpeed = 3f;
        private Vector3 _startPos;
        private Vector3 _handsStartPos;
        private CharacterController _controller;
        void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _startPos = camera.localPosition;
            _handsStartPos = hands.localPosition;
        }

        private void PlayMotion(Vector3 motion)
        {
            camera.localPosition += motion;
            hands.localPosition += motion;
        }
        private void CheckMotion()
        {
            var velocity = _controller.velocity;
            float speed = new Vector3(velocity.x, 0, velocity.z).magnitude;

            ResetPosition();
            if(speed < _toggleSpeed) return;
            if(!FirstPersonController.Instance.isGrounded)
            {
                return;
            }
            PlayMotion(FootStepMotion());
        }
        
        private void ResetPosition(){
            if(camera.localPosition == _startPos && hands.localPosition == _handsStartPos) return;
            camera.localPosition = Vector3.Lerp(camera.localPosition, _startPos, 3 * Time.deltaTime);
            hands.localPosition = Vector3.Lerp(hands.localPosition, _handsStartPos, 3 * Time.deltaTime);
        }
        private Vector3 FootStepMotion()
        {
            Vector3 pos = Vector3.zero;
            pos.y += Mathf.Sin(Time.time * frequency) * amplitude;
            pos.x = Mathf.Cos(Time.time * frequency / 2) * amplitude * 2;
            return pos;
        }

        private Vector3 FocusTarget()
        {
            var position = transform.position;
            Vector3 pos = new Vector3(position.x, position.y + cameraHolder.localPosition.y,
                position.z);
            pos += cameraHolder.forward * 15f;
            return pos;
        }
        void Update()
        {
            if(!enableBob) return;
            
            CheckMotion();
            camera.LookAt(FocusTarget());
        }
    }
}
