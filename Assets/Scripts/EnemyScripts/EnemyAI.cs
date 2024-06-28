using System.Collections;
using System.Collections.Generic;
using FPC;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using Random = UnityEngine.Random;

namespace EnemyScripts
{
    public class EnemyAI : MonoBehaviour
    {
        public static EnemyAI Instance { get; private set; }
        public NavMeshAgent aiNavMesh;
        public List<Transform> destinations;
        public Animator aiAnimator;
        public float walkSpeed, runSpeed, minIdleTime, maxIdleTime;
        private float _idleTime;
        public bool isWalking, isRunning;
        public Transform playerTransform;
        private Transform _currentDestination;
        private Vector3 _lastKnownPosition;
        public Vector3 destination;
        private int _randNum;
        private int _destinationsAmount;
        public bool isChasing;
        private static readonly int IsWalking = Animator.StringToHash("isWalking");
        private static readonly int IsRunning = Animator.StringToHash("isRunning");
        [Header("Enemy FOV")]
        public float radius;
        [Range(0, 360)] public float angle;
        public GameObject playerRef;
        public LayerMask targetMask;
        public LayerMask obstructionMask;
        public bool canSeePlayer;
        public bool radiusChanged;
        private Coroutine _lostPlayerCoroutine;
        private Coroutine _returnToLastKnownPositionCoroutine;
        private Coroutine _stayIdleCoroutine;
        private Coroutine _returnToPatrolCoroutine;
        [Header("Rig")] 
        [SerializeField]private MultiAimConstraint _headAim;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            _headAim = GameObject.Find("HeadAim").GetComponent<MultiAimConstraint>();
            aiAnimator = GetComponent<Animator>();
            _destinationsAmount = destinations.Count;
            SetNewDestination();
            isWalking = true;
            playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            // FOV
            playerRef = GameObject.FindGameObjectWithTag("Player");
            StartCoroutine(FOVRoutine());
        }

        private void Update()
        {
            aiAnimator.SetBool(IsWalking, isWalking);
            aiAnimator.SetBool(IsRunning, isRunning);
            FovRadiusController();
            FovAngleController();
            StartChase();
            if (isWalking)
            {
                if (aiNavMesh.remainingDistance <= aiNavMesh.stoppingDistance && !aiNavMesh.pathPending)
                {
                    HandleDestinationReached();
                }
            }
        }

        
        private void FovAngleController()
        {
            if (FirstPersonController.Instance.isSprinting && !isChasing)
            {
                angle = 360f;
            }
            else
            {
                angle = 100f;
            }
            
        }
        private void FovRadiusController()
        {
            if (FirstPersonController.Instance.isCrouching && !isChasing && !radiusChanged)
            {
                radius = 4f;
            }
            else if(!FirstPersonController.Instance.isCrouching && !isChasing && !radiusChanged)
            {
                radius = 8f;
            }
        }
        
        
        private void StartChase()
        {
            if (canSeePlayer)
            {
                isWalking = false;
                isRunning = true;
                isChasing = true;
                _headAim.weight = 1f;
                if (!radiusChanged)
                {
                    radius += 5;
                    radiusChanged = true;
                }
            }

            if (isChasing)
            {
                Chase();
            }
        }

        private void Chase()
        {
            if (isChasing)
            {
                var position = playerTransform.position;
                destination = position;
                aiNavMesh.SetDestination(destination);
                Vector3 direction = position - gameObject.transform.position;
                direction.y = 0;
                gameObject.transform.rotation = Quaternion.LookRotation(direction);
                aiNavMesh.speed = runSpeed;

                if (!canSeePlayer)
                {
                    _headAim.weight = 0f;
                    _lostPlayerCoroutine ??= StartCoroutine(LostPlayerCoroutine());
                }
            }
        }

        private IEnumerator LostPlayerCoroutine()
        {
            yield return new WaitForSeconds(2f);
            if (!canSeePlayer)
            {
                _lastKnownPosition = playerTransform.position;
                if (_returnToLastKnownPositionCoroutine != null)
                {
                    StopCoroutine(_returnToLastKnownPositionCoroutine);
                }

                _returnToLastKnownPositionCoroutine = StartCoroutine(ReturnToLastKnownPosition());
                isChasing = false;
            }
            _lostPlayerCoroutine = null;
        }
        private IEnumerator ReturnToLastKnownPosition()
        {
            aiNavMesh.SetDestination(_lastKnownPosition);
            while (Vector3.Distance(transform.position, _lastKnownPosition) > aiNavMesh.stoppingDistance)
            {
                yield return null;
            }
            isWalking = false;
            isRunning = false;
            if (radiusChanged)
            {
                radius -= 5;
                radiusChanged = false;
            }
            if (!canSeePlayer)
            {
                if (_stayIdleCoroutine != null)
                {
                    StopCoroutine(_stayIdleCoroutine);
                }
                _stayIdleCoroutine = StartCoroutine(StayIdle());
            }
            else
            {
                if (_stayIdleCoroutine != null)
                {
                    StopCoroutine(_stayIdleCoroutine);
                }
                if (_returnToPatrolCoroutine != null)
                {
                    StopCoroutine(_returnToPatrolCoroutine);
                }
                StartChase();
            }
        }

        private IEnumerator ReturnToPatrol()
        {
            SetNewDestination();
            yield return null;
        }

        private void CheckForPlayer()
        {
            Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
            if (rangeChecks.Length != 0)
            {
                Transform target = rangeChecks[0].transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);
                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    {
                        canSeePlayer = true;
                    }
                    else
                    {
                        canSeePlayer = false;
                    }
                }
                else
                {
                    canSeePlayer = false;
                }
            }
            else if (canSeePlayer)
            {
                canSeePlayer = false;
            }
        }

        private IEnumerator FOVRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(0.2f);
            while (true)
            {
                yield return wait;
                CheckForPlayer();
            }
        }

        private void SetNewDestination()
        {
            int newDestinationIndex;
            do
            {
                newDestinationIndex = Random.Range(0, _destinationsAmount);
            } while (destinations[newDestinationIndex] == _currentDestination);

            _randNum = newDestinationIndex;
            _currentDestination = destinations[_randNum];
            destination = _currentDestination.position;
            aiNavMesh.destination = destination;
            aiNavMesh.speed = walkSpeed;
        }

        private void HandleDestinationReached()
        {
            int randomChoice = Random.Range(0, 2);
            if (randomChoice == 0)
            {
                SetNewDestination();
                isWalking = true;
            }
            else
            {
                if (_stayIdleCoroutine != null)
                {
                    StopCoroutine(_stayIdleCoroutine);
                }
                _stayIdleCoroutine = StartCoroutine(StayIdle());
                isWalking = false;
            }
        }

        private IEnumerator StayIdle()
        {
            _idleTime = Random.Range(minIdleTime, maxIdleTime);
            yield return new WaitForSeconds(_idleTime);
            isWalking = true;
            SetNewDestination();
            Debug.Log($"Idle for: {_idleTime}");
        }
    }
}
