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

        private float _toggleSpeed = 3f;
        private Vector3 _startPos;
        private CharacterController _controller;
        void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _startPos = camera.localPosition;
        }

        private void PlayMotion(Vector3 motion)
        {
            camera.localPosition += motion;
        }
        private void CheckMotion()
        {
            float speed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;

            ResetPosition();
            if(speed < _toggleSpeed) return;
            if(!_controller.isGrounded)
            {
                return;
            }

            PlayMotion(FootStepMotion());
        }
        
        private void ResetPosition(){
            if(camera.localPosition == _startPos) return;
            camera.localPosition = Vector3.Lerp(camera.localPosition, _startPos, 1 * Time.deltaTime);
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
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + cameraHolder.localPosition.y,
                transform.position.z);
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
