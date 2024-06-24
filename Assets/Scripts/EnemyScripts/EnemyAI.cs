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
        public Transform aiTransform;
        private Transform _currentDestination;
        private Vector3 _destination;
        private int _randNum, _randNum2;
        private int _destinationsAmount;
        //Raycast
        public Vector3 rayCastOffset;
        public float sightDistance;
        private static readonly int IsWalking = Animator.StringToHash("isWalking");

        private void Start()
        {
            aiAnimator = GetComponent<Animator>();
            _destinationsAmount = destinations.Count;
            SetNewDestination();
            isWalking = true;
        }

        private void Update()
        {
            aiAnimator.SetBool(IsWalking, isWalking);
            CheckForPlayer();
            if (isWalking)
            {
                if (aiNavMesh.remainingDistance <= aiNavMesh.stoppingDistance && !aiNavMesh.pathPending)
                {
                    HandleDestinationReached();
                }
            }
        }


        private void CheckForPlayer()
        {
            Vector3 aiDirection = (aiTransform.position - transform.position).normalized;
            RaycastHit hit;
            Debug.DrawRay(transform.position + rayCastOffset, aiDirection * sightDistance, Color.red);
            if (Physics.Raycast(transform.position + rayCastOffset, aiDirection, out hit, sightDistance))
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    Debug.Log("Player on raycast");
                }
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
