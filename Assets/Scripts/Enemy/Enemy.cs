using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private EnemyMod[] activeMods;

    [Header("Enemy Settings")]
    [SerializeField] private float health = 50f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private float attackCooldownS = 2f;
    [SerializeField] private float detectionRadius = 5f;
    private SphereCollider sphereCollider;

    [Header("Navigation")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float angularSpeed = 120f;
    [SerializeField] private bool autoBraking = false;
    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    private bool playerInRadius = false;
    private Transform player;

    [Header("AI Settings")]
    [SerializeField] private float pathUpdateInterval = 0.2f;
    private float pathUpdateTimer = 0f;

    protected void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogWarning("NavMeshAgent component not found!");
            return;
        }

        agent.speed = speed;
        agent.acceleration = acceleration;
        agent.angularSpeed = angularSpeed;
        agent.autoBraking = autoBraking;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) player = playerObject.transform;

        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }

        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = detectionRadius;

        //Mod loader
        activeMods = GetComponents<EnemyMod>();
        foreach (var mod in activeMods)
        {
            mod.Apply(this);
        }
    }

    protected void Update()
    {
        pathUpdateTimer += Time.deltaTime;

        if (playerInRadius && player != null && pathUpdateTimer >= pathUpdateInterval)
        {
            agent.SetDestination(player.position);
            pathUpdateTimer = 0f;
        }
        else if (!playerInRadius)
        {
            Patrol();
        }

        foreach (var mod in activeMods) mod.ModUpdate(this);
    }

    private void Patrol()
    {
        if (waypoints.Length == 0) return;

        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRadius = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRadius = false;
        }
    }

    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f) Die();
    }

    protected virtual void Die()
    {
        Debug.Log("Enemy died.");
        Destroy(gameObject);
    }

    public float GetDamage() => damage;
    public float GetAttackCooldownS() => attackCooldownS;
}
