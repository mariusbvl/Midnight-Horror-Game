using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace EnemyScripts
{
    public class EnemyAI : MonoBehaviour
    {
        public NavMeshAgent aiNavMesh;
        public List<Transform> destinations;
        public Animator aiAnimator;
        public float walkSpeed, runSpeed, minIdleTime, maxIdleTime;
        private float _idleTime;
        public bool isWalking, isRunning;
        public Transform playerTransform;
        private Transform _currentDestination;
        private Vector3 _lastKnownPosition;
        private Vector3 _destination;
        private int _randNum, _randNum2;
        private int _destinationsAmount;
        public bool isChasing;
        private static readonly int IsWalking = Animator.StringToHash("isWalking");
        private static readonly int IsRunning = Animator.StringToHash("isRunning");
        [Header("Enemy FOV")]
        public float radius;
        [Range(0, 360)] public float angle;
        public GameObject playerRef;
        public LayerMask targetMask;
        public LayerMask obsructionMask;
        public bool canSeePlayer;
        

        private void Start()
        {
            aiAnimator = GetComponent<Animator>();
            _destinationsAmount = destinations.Count;
            SetNewDestination();
            isWalking = true;
            playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            //FOV
            playerRef = GameObject.FindGameObjectWithTag("Player");
            StartCoroutine(FOVRoutine());
        }

        private void Update()
        {
            aiAnimator.SetBool(IsWalking, isWalking);
            aiAnimator.SetBool(IsRunning, isRunning);
            StartChase();
            if (isWalking)
            {
                if (aiNavMesh.remainingDistance <= aiNavMesh.stoppingDistance && !aiNavMesh.pathPending)
                {
                    HandleDestinationReached();
                }
            }
        }
        
        private void StartChase(){
            if (canSeePlayer)
            {
                isWalking = false;
                isRunning = true;
                isChasing = true;
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
                _destination = position;
                aiNavMesh.SetDestination(_destination);
                Vector3 direction = position - gameObject.transform.position;
                direction.y = 0;
                gameObject.transform.rotation = Quaternion.LookRotation(direction);
                aiNavMesh.speed = runSpeed;
                if (!canSeePlayer)
                {
                    _lastKnownPosition = playerTransform.position;
                    StartCoroutine(ReturnToLastKnownPosition());
                    isChasing = false;
                }
            }
        }
        private IEnumerator ReturnToLastKnownPosition()
        {
            aiNavMesh.SetDestination(_lastKnownPosition);
            while (Vector3.Distance(gameObject.transform.position, _lastKnownPosition) > aiNavMesh.stoppingDistance)
            {
                yield return null;
            }
            isWalking = true;
            isRunning = false;
            StopCoroutine(StayIdle());
            yield return StartCoroutine(StayIdle());
            if (!canSeePlayer)
            {
                StartCoroutine(ReturnToPatrol());
            }
            else
            {
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
                if (Vector3.Angle(transform.forward , directionToTarget) < angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);
                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obsructionMask))
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
            else if(canSeePlayer)
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
            _destination = _currentDestination.position;
            aiNavMesh.destination = _destination;
            aiNavMesh.speed = walkSpeed;
        }

        private void HandleDestinationReached()
        {
            _randNum2 = Random.Range(0, 2);
            if (_randNum2 == 0)
            {
                SetNewDestination();
            }
            else
            {
                StartCoroutine(StayIdle());
                isWalking = false;
            }
        }

        private IEnumerator StayIdle()
        {
            _idleTime = Random.Range(minIdleTime, maxIdleTime);
            yield return new WaitForSeconds(_idleTime);
            isWalking = true;
            SetNewDestination();
        }
    }
}
