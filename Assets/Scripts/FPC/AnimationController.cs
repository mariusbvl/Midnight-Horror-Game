using UnityEngine;

namespace FPC
{
    public class AnimationController : MonoBehaviour
    {
        public static AnimationController Instance {get; private set; }
        [SerializeField] private FirstPersonController fpc;
        [HideInInspector]public Animator animator;
        private bool _inAirIdle;
        private bool _inAirMoving;
        private static readonly int IsWalking = Animator.StringToHash("isWalking");
        private static readonly int IsWalkingForward = Animator.StringToHash("isWalkingForward");
        private static readonly int IsWalkingBackwards = Animator.StringToHash("isWalkingBackwards");
        private static readonly int IsStrafingRight = Animator.StringToHash("isStrafingRight");
        private static readonly int IsStrafingLeft = Animator.StringToHash("isStrafingLeft");
        private static readonly int IsRunning = Animator.StringToHash("isRunning");
        private static readonly int Crouch = Animator.StringToHash("Crouch");
        private static readonly int IsCrouching = Animator.StringToHash("isCrouching");
        private static readonly int Stand = Animator.StringToHash("Stand");
        private static readonly int MovingJump = Animator.StringToHash("MovingJump");
        private static readonly int MovingLanded = Animator.StringToHash("MovingLanded");
        private static readonly int IdleJump = Animator.StringToHash("IdleJump");
        private static readonly int IdleLanded = Animator.StringToHash("IdleLanded");
        private static readonly int InAirMoving = Animator.StringToHash("inAirMoving");
        private static readonly int InAirIdle = Animator.StringToHash("inAirIdle");


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            animator = GetComponent<Animator>();
        }
        
        void Update()
        {
            WalkingAnimationController();
            RunningAnimationController();
            IdleJumpingAnimationController();
            MovingJumpingAnimationController();
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


        private void MovingJumpingAnimationController()
        {
            
            if (fpc.inputActions.Player.Move.inProgress && !fpc.isJumpIdle)
            {
                _inAirMoving = fpc.isInAir;
                if (_inAirMoving)
                {
                    animator.SetBool(InAirMoving,true);
                    animator.SetTrigger(MovingJump);
                }
                else
                {
                    animator.SetBool(InAirMoving,false);
                    animator.SetTrigger(MovingLanded);
                }
            }
        }
        
        private void IdleJumpingAnimationController()
        {
            if (!fpc.inputActions.Player.Move.inProgress && !_inAirMoving)
            {
                _inAirIdle = fpc.isInAir;
                if (_inAirIdle)
                {
                    animator.SetBool(InAirIdle, true);
                    animator.SetTrigger(IdleJump);
                }
                else
                {
                    animator.SetBool(InAirIdle, false);
                    animator.SetTrigger(IdleLanded);
                }
            }
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
