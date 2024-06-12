using UnityEngine;

namespace FPC
{
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] private FirstPersonController fpc;
        [HideInInspector]public Animator animator;
        private bool _wasGrounded;
        private static readonly int IsWalking = Animator.StringToHash("isWalking");
        private static readonly int IsWalkingForward = Animator.StringToHash("isWalkingForward");
        private static readonly int IsWalkingBackwards = Animator.StringToHash("isWalkingBackwards");
        private static readonly int IsStrafingRight = Animator.StringToHash("isStrafingRight");
        private static readonly int IsStrafingLeft = Animator.StringToHash("isStrafingLeft");
        private static readonly int IsRunning = Animator.StringToHash("isRunning");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int Landed = Animator.StringToHash("Landed");
        private static readonly int Crouch = Animator.StringToHash("Crouch");
        private static readonly int IsCrouching = Animator.StringToHash("isCrouching");
        private static readonly int Stand = Animator.StringToHash("Stand");


        void Awake()
        {
            animator = GetComponent<Animator>();
        }
        
        void Update()
        {
            WalkingAnimationController();
            RunningAnimationController();
            JumpingAnimationController();
            CrouchAnimationController();
        }

        private void CrouchAnimationController()
        {
            if (fpc.inputActions.Player.Crouch.triggered && fpc.isCrouching)
            {
                animator.SetTrigger(Crouch);
                animator.SetBool(IsCrouching, true);
            }

            if (fpc.inputActions.Player.Crouch.triggered && !fpc.isCrouching)
            {
                animator.SetTrigger(Stand);
                animator.SetBool(IsCrouching, false);
            }
        }
        
        
        
        private void JumpingAnimationController(){
            if (fpc.inputActions.Player.Jump.triggered && fpc.isGrounded)
            {
                animator.SetTrigger(Jump);
            }
            if(!_wasGrounded && fpc.isGrounded)
            {
                animator.SetTrigger(Landed);
            }
            _wasGrounded = fpc.isGrounded;
        }
        private void RunningAnimationController()
        {
                if (fpc.inputActions.Player.Move.inProgress && fpc.inputActions.Player.Sprint.inProgress)
                {
                    animator.SetBool(IsRunning, true);
                }
                else
                {
                    animator.SetBool(IsRunning, false);
                }
        }
        
        private void WalkingAnimationController()
        {
            
            if (fpc.inputActions.Player.Move.inProgress)
            {
                WalkingDirection();
                animator.SetBool(IsWalking, true);
                
            }
            else
            {
                WalkingReset();
            }
        }

        private void WalkingDirection()
        {
            if (fpc.move.y > 0)
            {
                animator.SetBool(IsWalkingForward, true);
            }else if (fpc.move.y < 0)
            {
                animator.SetBool(IsWalkingBackwards, true);
            }
            else
            {
                animator.SetBool(IsWalkingForward, false);
                animator.SetBool(IsWalkingBackwards, false);
            }

            if (fpc.move.x > 0)
            {
                animator.SetBool(IsStrafingRight, true);
            }else if (fpc.move.x < 0)
            {
                animator.SetBool(IsStrafingLeft, true);
            }
            else
            {
                animator.SetBool(IsStrafingRight, false);
                animator.SetBool(IsStrafingLeft, false);
            }
        }
        
        private void WalkingReset()
        {
            if (!fpc.inputActions.Player.Move.inProgress)
            {
                animator.SetBool(IsWalking, false);
                animator.SetBool(IsWalkingForward, false);
                animator.SetBool(IsWalkingBackwards, false);
                animator.SetBool(IsStrafingRight, false);
                animator.SetBool(IsStrafingLeft, false);
            }
        }
    }
}
