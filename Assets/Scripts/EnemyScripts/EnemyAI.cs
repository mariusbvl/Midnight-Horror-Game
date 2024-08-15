using System.Collections;
using System.Collections.Generic;
using FPC;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;
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
        [FormerlySerializedAs("_headAim")]
        [Header("Rig")] 
        [SerializeField]private MultiAimConstraint headAim;

        [Header("Audio")] 
        [SerializeField] private AudioClip monsterIdleSound;
        [SerializeField] private AudioClip monsterStartChaseSound;
        [SerializeField] private AudioClip monsterChasingPeriodicRoarSound;
        private AudioSource _monsterIdleAudioSource;
        private bool _startChaseSoundPlayed;
        private bool _idleSoundIsPlaying;
        private bool isRoarCoroutineRunning;
        private bool isRoarPlaying;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            headAim = GameObject.Find("HeadAim").GetComponent<MultiAimConstraint>();
            aiAnimator = GetComponent<Animator>();
            _destinationsAmount = destinations.Count;
            SetNewDestination();
            isWalking = true;
            playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            // FOV
            playerRef = GameObject.FindGameObjectWithTag("Player");
            StartCoroutine(FOVRoutine());
            PlayIdleRoar();
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

            if (isChasing && !isRoarCoroutineRunning)
            {
                StartCoroutine(RoarWhileChasing());
            }
            else if (!isChasing && isRoarCoroutineRunning)
            {
                StopCoroutine(RoarWhileChasing());
                isRoarCoroutineRunning = false;
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
                //StopIdleRoar();
                if (!_startChaseSoundPlayed)
                {
                    SoundFXManager.Instance.PlaySoundFxClip(monsterStartChaseSound, gameObject.transform, 1f, 1f);
                    _startChaseSoundPlayed = true;
                }
                isWalking = false;
                isRunning = true;
                isChasing = true;
                headAim.weight = 1f;
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
            if (!isChasing) return;
            var position = playerTransform.position;

            // Attempt to set the destination
            bool pathSet = aiNavMesh.SetDestination(position);

            if (!pathSet || aiNavMesh.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                Debug.Log("Invalid path");

                // Handle the invalid path scenario, maybe stop chasing or perform other actions
                headAim.weight = 0f;

                // Set the last known position and stop chasing
                _lastKnownPosition = FirstPersonController.Instance.isHidden ? 
                    InteractController.Instance.currentFrontOfTheLockerPoint.transform.position : 
                    playerTransform.position;

                if (_returnToLastKnownPositionCoroutine != null)
                {
                    StopCoroutine(_returnToLastKnownPositionCoroutine);
                }

                _returnToLastKnownPositionCoroutine = StartCoroutine(ReturnToLastKnownPosition());
                isChasing = false;
                return;
            }

            // If the path is valid, continue chasing
            aiNavMesh.speed = runSpeed;

            if (canSeePlayer) return;
            headAim.weight = 0f;
            _lostPlayerCoroutine ??= StartCoroutine(LostPlayerCoroutine());
        }

        private IEnumerator LostPlayerCoroutine()
        {
            if (!FirstPersonController.Instance.isHidden)
            {
                yield return new WaitForSeconds(1.5f);
            }
            if (!canSeePlayer )
            {
                _lastKnownPosition = FirstPersonController.Instance.isHidden ? InteractController.Instance.currentFrontOfTheLockerPoint.transform.position : playerTransform.position;

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
        

        private void CheckForPlayer()
        {
            Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
            if (rangeChecks.Length != 0)
            {
                Transform target = rangeChecks[0].transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                {
                    var position = transform.position;
                    float distanceToTarget = Vector3.Distance(position, target.position);
                    canSeePlayer = !Physics.Raycast(position, directionToTarget, distanceToTarget, obstructionMask);
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

        private void  SetNewDestination()
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
        }
        

        private void PlayIdleRoar()
        {
            if(_idleSoundIsPlaying) return;
            SoundFXManager.Instance.PlaySoundFxClip(monsterIdleSound, gameObject.transform, 1f,1f,true,3600);
            _monsterIdleAudioSource = SoundFXManager.Instance.audioSource;
            _monsterIdleAudioSource.transform.SetParent(gameObject.transform);
            _idleSoundIsPlaying = true;
        }

        private void StopIdleRoar()
        {
            if (!(isChasing && _idleSoundIsPlaying)) return;
            Destroy(_monsterIdleAudioSource.gameObject);
            _idleSoundIsPlaying = false;
        }
        
        private IEnumerator RoarWhileChasing()
        {
            isRoarCoroutineRunning = true;

            while (isChasing)
            {
                if (!isRoarPlaying)
                {
                    float randomTime = Random.Range(5f, 10f);
                    yield return new WaitForSeconds(randomTime);

                    if (isChasing)
                    {
                        // Play the roar sound once
                        SoundFXManager.Instance.PlaySoundFxClip(monsterChasingPeriodicRoarSound, transform, 1f, 1f);
                        isRoarPlaying = true;

                        // Reset the roar flag after a delay to allow another roar
                        float delayTime = Random.Range(5f, 10f);
                        yield return new WaitForSeconds(delayTime);

                        // Ensure the coroutine doesn't continue if the chase stops
                        if (!isChasing) break;

                        isRoarPlaying = false;
                    }
                }
                else
                {
                    yield return null; // Wait for the next frame if the sound is playing
                }
            }

            isRoarCoroutineRunning = false;
        }
    }
}
