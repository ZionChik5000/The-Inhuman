using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private Transform[] waypoints;
    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    private bool playerInRadius = false;
    private Transform player;

    [Header("Enemy Settings")]
    [SerializeField] private float health = 50f;
    //[SerializeField] private float damage = 10f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player object not found. Ensure the player has the tag 'Player'.");
        }

        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    void Update()
    {
        if (!playerInRadius)
        {
            Patrol();
        }
        else if (player != null)
        {
            FollowPlayer();
        }
    }

    private void Patrol()
    {
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    private void FollowPlayer()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
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

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"Enemy took damage: {amount}. Current health: {health}");
        if (health <= 0f)
        {
            Die();
        }
        else
        {
            OnDamageTaken();
        }
    }

    private void OnDamageTaken()
    {
        Debug.Log("Enemy took damage but is still alive.");
    }

    private void Die()
    {
        Debug.Log("Enemy died.");
        Destroy(gameObject);
    }
}
