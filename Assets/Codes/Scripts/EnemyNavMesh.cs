using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMesh : MonoBehaviour
{
    public LayerMask whatIsGround, whatIsPlayer;

    public GameObject bulletPrefab;
    public Transform bulletSpawnpoint;

    // When Patroling
    public Vector3 walkPoint;
    private bool _walkPointSetted;
    public float walkPointRange;

    // When Attacking
    public float attackCooldown;
    private bool _attackOnCooldown;

    // Agent State
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private NavMeshAgent _agent;
    [SerializeField] private Transform _player;

    private void Awake()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            //check for sight and attack range
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            RaycastDetector();

            if (!playerInSightRange && !playerInAttackRange) Patroling();
            else if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            else if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }
    }

    private void RaycastDetector()
    {
        RaycastHit hit;

        // If hit wall, search new point
        if (Physics.Raycast(transform.position, transform.forward, out hit, sightRange))
        {
            if(hit.transform.gameObject.CompareTag("Wall"))
            {
                SearchWalkPoint();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 5f);

        Gizmos.DrawWireSphere(transform.position, sightRange);
    }


    private void Patroling()
    {
        if (!_walkPointSetted) SearchWalkPoint();

        if (_walkPointSetted)
            _agent.SetDestination(walkPoint);

        transform.LookAt(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // When agent reach the WalkPoint
        if (distanceToWalkPoint.magnitude < 1f)
            _walkPointSetted = false;
    }

    private void SearchWalkPoint()
    {
        // Create and calculate two random point in range
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        float randomZ = Random.Range(-walkPointRange, walkPointRange);

        // Randomize where do the agent walk
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            _walkPointSetted = true;

    }

    private void ChasePlayer()
    {
        transform.LookAt(_player);
        if (Vector3.Distance(transform.position, _player.position) <= sightRange)
        {
            _agent.SetDestination(_player.position);
        }
        else
        {
            playerInSightRange = false;
        }
    }

    private void AttackPlayer()
    {
        transform.LookAt(_player);
        if (Vector3.Distance(transform.position, _player.position) <= attackRange)
        {
            _agent.isStopped = true;
            playerInAttackRange = true;
        }
        else
        {
            playerInAttackRange = false;
            _agent.isStopped = false;
            _agent.SetDestination(_player.position);
        }

        if (!_attackOnCooldown)
        {
            //attack code here
            Rigidbody bullet = Instantiate(bulletPrefab, bulletSpawnpoint.position, Quaternion.identity).GetComponent<Rigidbody>();
            bullet.velocity = transform.forward * 50f;

            _attackOnCooldown = true;
            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }

    private void ResetAttack()
    {
        _attackOnCooldown = false;
    }
}
