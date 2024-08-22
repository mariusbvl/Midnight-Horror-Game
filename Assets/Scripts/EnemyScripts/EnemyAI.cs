using System;
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
        public static EnemyAI Instance { get; set; }
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
        public bool isPatrolling;
        private Coroutine fovRoutine;
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
        public bool playerOnInvalidPath;
        [Header("Rig")] 
        [SerializeField]private MultiAimConstraint headAim;

        [Header("Doors")] 
        [SerializeField] private CapsuleCollider enemyCapsuleCollider;
        [SerializeField] private BoxCollider[] doorColliders;
        [SerializeField] private float aiDoorOpenDuration;
        public GameObject currentDoorParent;
        public Transform currentDoorPivot;
        public Transform currentDoorClosedPivot;
        public Transform currentDoorOpenPivot;
        
        
        [Header("Audio")] 
        [SerializeField] private AudioClip monsterIdleSound;
        [SerializeField] private AudioClip monsterStartChaseSound;
        [SerializeField] private AudioClip monsterChasingPeriodicRoarSound;
        [SerializeField] private AudioClip doorOpeningSound;
        private AudioSource _monsterIdleAudioSource;
        private AudioSource _monsterStartChaseAudioSource;
        private AudioSource _monsterChasingRoarAudioSource;
        private bool _startChaseSoundPlayed;
        public bool _idleSoundIsPlaying;
        private bool _isRoarCoroutineRunning;
        private bool _isRoarPlaying;

        private void Start()
        {
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("AiDoorCollider");
            doorColliders = new BoxCollider[objectsWithTag.Length];
            for (int i = 0; i < objectsWithTag.Length; i++)
            {
                doorColliders[i] = objectsWithTag[i].GetComponent<BoxCollider>();
            }
            
            headAim = GameObject.Find("HeadAim").GetComponent<MultiAimConstraint>();
            aiAnimator = GetComponent<Animator>();
            _destinationsAmount = destinations.Count;
            SetNewDestination();
            isWalking = true;
            playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            // FOV
            playerRef = GameObject.FindGameObjectWithTag("Player");
        }

        private void Update()
        {
            //Animation and fov handling
            aiAnimator.SetBool(IsWalking, isWalking);
            aiAnimator.SetBool(IsRunning, isRunning);
            FovRadiusAndAngleController();
            
            CheckForValidPath();
            
            //Patroling
            if (isWalking)
            {
                if (aiNavMesh.remainingDistance <= aiNavMesh.stoppingDistance && !aiNavMesh.pathPending)
                {
                    HandleDestinationReached();
                }
            }
            
            
            //Periodic Roar
            if (isChasing && !_isRoarCoroutineRunning)
            {
                StartCoroutine(RoarWhileChasing());
            }
            else if (!isChasing && _isRoarCoroutineRunning)
            {
                StopCoroutine(RoarWhileChasing());
                _isRoarCoroutineRunning = false;
            }
            
            //Start the Chase
            StartChase();
            
            //Chase
            if (isChasing && !playerOnInvalidPath)
            {
                Chase();
            }
            
            AiOpenDoor();
            
        }

        private void CheckForValidPath()
        {
            NavMeshPath path = new NavMeshPath();
            aiNavMesh.CalculatePath(playerTransform.position, path);

            // Check if the path is partial
            playerOnInvalidPath = path.status == NavMeshPathStatus.PathPartial;
        }
        private void FovRadiusAndAngleController()
        {
            if (FirstPersonController.Instance.isCrouching && !isChasing )
            {
                radius = 4f;
                angle = 100f;
            }
            else if(!FirstPersonController.Instance.isCrouching && !FirstPersonController.Instance.isSprinting && !isChasing)
            {
                radius = 8f;
                angle = 100f;
            }
            else if (FirstPersonController.Instance.isSprinting && !isChasing)
            {
                radius = 8f;
                angle = 360f;
            }
            else if (isChasing)
            {
                radius = 16f;
                angle = 100f;
            }
        }
        
        
        private void StartChase()
        {
            if (canSeePlayer)
            {
                if (!_startChaseSoundPlayed)
                {
                    SoundFXManager.Instance.PlaySoundFxClip(monsterStartChaseSound, gameObject.transform, 1f, 1f);
                    _monsterStartChaseAudioSource = SoundFXManager.Instance.audioSource;
                    _monsterStartChaseAudioSource.transform.SetParent(gameObject.transform);
                    _startChaseSoundPlayed = true;
                }
                isWalking = false;
                isRunning = true;
                isChasing = true;
                isPatrolling = false;
                headAim.weight = 1f;
            }
        }

        private void Chase()
        {
            if (!isChasing) return;
            
            
            //Player on invalidPath
            if (aiNavMesh.pathStatus == NavMeshPathStatus.PathPartial)
            {
                Debug.Log("Player on Partial Invalid Path");
                playerOnInvalidPath = true;
                canSeePlayer = false;
                headAim.weight = 0f;
                _lostPlayerCoroutine ??= StartCoroutine(LostPlayerCoroutine());
            }

            if (FirstPersonController.Instance.isHidden)
            {
                headAim.weight = 0f;
                _lostPlayerCoroutine ??= StartCoroutine(LostPlayerCoroutine());
            }
            

            if (!canSeePlayer)
            {
                headAim.weight = 0f;
                _lostPlayerCoroutine ??= StartCoroutine(LostPlayerCoroutine());
            }
            
            aiNavMesh.speed = runSpeed;
            aiNavMesh.SetDestination(playerTransform.position);
        }
        
        
        private IEnumerator LostPlayerCoroutine()
        {
            if (!FirstPersonController.Instance.isHidden && playerOnInvalidPath)
            {
                yield return new WaitForSeconds(1.5f);
            }

            if (_returnToLastKnownPositionCoroutine != null)
            {
                StopCoroutine(_returnToLastKnownPositionCoroutine);
            }

            _returnToLastKnownPositionCoroutine = StartCoroutine(ReturnToLastKnownPosition(SetLastKnownPosition()));
            isChasing = false;
            _lostPlayerCoroutine = null;
        }

        private Vector3 SetLastKnownPosition()
        {
            if (FirstPersonController.Instance.isHidden)
            {
                _lastKnownPosition = InteractController.Instance.currentFrontOfTheLockerPoint.transform.position;
            }
            else if (playerOnInvalidPath)
            {
                _lastKnownPosition = transform.position; 
            }
            else
            {
                _lastKnownPosition = playerTransform.position; 
            }
            return _lastKnownPosition;
        }
        
        private IEnumerator ReturnToLastKnownPosition(Vector3 lastKnownPos)
        {
            aiNavMesh.SetDestination(lastKnownPos);
            while (Vector3.Distance(transform.position, lastKnownPos) > aiNavMesh.stoppingDistance)
            {
                yield return null;
            }
            isWalking = false;
            isRunning = false;
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
            if(playerOnInvalidPath) return;
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
            isPatrolling = true;
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

        private IEnumerator  StayIdle()
        {
            _idleTime = Random.Range(minIdleTime, maxIdleTime);
            yield return new WaitForSeconds(_idleTime);
            isWalking = true;
            SetNewDestination();
        }


        private void AiOpenDoor()
        {
            foreach (BoxCollider doorColliderObject in doorColliders)
            {
                if (enemyCapsuleCollider.bounds.Intersects(doorColliderObject.bounds))
                { 
                    currentDoorPivot = doorColliderObject.transform.parent;
                    currentDoorParent = currentDoorPivot.transform.parent.gameObject;
                    currentDoorOpenPivot = currentDoorParent.transform.Find("openDoor");
                    currentDoorClosedPivot = currentDoorParent.transform.Find("closedDoor");
                    if (currentDoorPivot.rotation == currentDoorClosedPivot.rotation)
                    {
                        SoundFXManager.Instance.PlaySoundFxClip(doorOpeningSound, currentDoorPivot, 1f,1f);
                        StartCoroutine(AiOpenDoorCoroutine(currentDoorPivot, currentDoorOpenPivot));
                    }
                }
            }
        }

        private IEnumerator AiOpenDoorCoroutine(Transform doorPivot, Transform openPivot)
        {
            Quaternion startRotation = doorPivot.rotation;
            Quaternion targetRotation = openPivot.rotation;
            float timeElapsed = 0f;

            while (timeElapsed < aiDoorOpenDuration)
            {
                doorPivot.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / aiDoorOpenDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            doorPivot.transform.rotation = targetRotation; 
        }
        
        private void PlayIdleRoar()
        {
            if(_idleSoundIsPlaying) return;
            SoundFXManager.Instance.PlaySoundFxClip(monsterIdleSound, gameObject.transform, 1f,1f,true,3600);
            _monsterIdleAudioSource = SoundFXManager.Instance.audioSource;
            _monsterIdleAudioSource.transform.SetParent(gameObject.transform);
            _idleSoundIsPlaying = true;
            Debug.Log($"Audio Source is playing {_monsterIdleAudioSource.isPlaying}");
        }
        
        private void StopIdleRoar()
        {
            if (!_idleSoundIsPlaying) return;
            if (_monsterIdleAudioSource.gameObject != null)
            {
                Destroy(_monsterIdleAudioSource.gameObject);
            }
            _idleSoundIsPlaying = false;
        }
        
        private IEnumerator RoarWhileChasing()
        {
            _isRoarCoroutineRunning = true;

            while (isChasing)
            {
                if (!_isRoarPlaying)
                {
                    float randomTime = Random.Range(5f, 10f);
                    yield return new WaitForSeconds(randomTime);

                    if (isChasing)
                    {
                        SoundFXManager.Instance.PlaySoundFxClip(monsterChasingPeriodicRoarSound, transform, 1f, 1f);
                        _monsterChasingRoarAudioSource = SoundFXManager.Instance.audioSource;
                        _monsterChasingRoarAudioSource.transform.SetParent(gameObject.transform);
                        _isRoarPlaying = true;
                        
                        float delayTime = Random.Range(5f, 10f);
                        yield return new WaitForSeconds(delayTime);
                        
                        if (!isChasing) break;

                        _isRoarPlaying = false;
                    }
                }
                else
                {
                    yield return null;
                }
            }

            _isRoarCoroutineRunning = false;
        }

        private void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            fovRoutine = StartCoroutine(FOVRoutine());
            PlayIdleRoar();
        }

        private void OnDisable()
        {
            StopCoroutine(fovRoutine);
            StopIdleRoar();
            if (Instance == this)
            {
                Instance = null;
            }
            
        }
    }
}
