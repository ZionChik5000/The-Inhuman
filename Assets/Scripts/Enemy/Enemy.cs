using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Modification")]
    [SerializeField] private EnemyMod activeMod;


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

    [Header("Enemy Settings")]
    [SerializeField] private float health = 50f;
    [SerializeField] private float damage = 5f;

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
        else
        {
            agent.speed = speed;
            agent.acceleration = acceleration;
            agent.angularSpeed = angularSpeed;
            agent.autoBraking = autoBraking;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player object not found. Ensure the player has the tag 'Player'.");
        }

        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }

        ModStart();
    }

    protected void Update()
    {
        pathUpdateTimer += Time.deltaTime;

        if (!playerInRadius)
        {
            Patrol();
        }
        else if (playerInRadius && player != null && pathUpdateTimer >= pathUpdateInterval)
        {
            FollowPlayer();
            pathUpdateTimer = 0f;
        }

        ModUpdate();
    }

    protected virtual void Patrol()
    {
        if (waypoints.Length == 0) return;

        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }


    protected virtual void FollowPlayer()
    {
        if (player != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(player.position);
        }
    }

    protected virtual void Attack()
    {
        //Work in progress...
    }

    private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        playerInRadius = true;
        Debug.Log("Player in radius");
    }
}

private void OnTriggerExit(Collider other)
{
    if (other.CompareTag("Player"))
    {
        playerInRadius = false;
        Debug.Log("Player is NOT in radius");
    }
}


    public virtual void TakeDamage(float amount)
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

    protected virtual void OnDamageTaken()
    {
        Debug.Log("Enemy took damage but is still alive.");
    }

    protected virtual void Die()
    {
        Debug.Log("Enemy died.");
        Destroy(gameObject);
    }

    // ------------------   MODS SECTION   --------------------------------
    //DO NOT CHANGE HERE ANYTHING!!

    public virtual void ModStart()
    {
        if (activeMod != null)
        {
            activeMod.Apply(this);
        }
    }

    public virtual void ModUpdate()
    {
        if (activeMod != null)
        {
            activeMod.ModUpdate(this);
        }
    }




    public void SetHealth(float newHealth) => health = newHealth;
    public void SetDamage(float newDamage) => damage = newDamage;
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
        if (agent != null) agent.speed = speed;
    }
    public void SetAcceleration(float newAcceleration)
    {
        acceleration = newAcceleration;
        if (agent != null) agent.acceleration = acceleration;
    }
    public void SetAngularSpeed(float newAngularSpeed)
    {
        angularSpeed = newAngularSpeed;
        if (agent != null) agent.angularSpeed = angularSpeed;
    }
    public void SetAutoBraking(bool newAutoBraking)
    {
        autoBraking = newAutoBraking;
        if (agent != null) agent.autoBraking = autoBraking;
    }

    public float GetHealth() => health;
    public float GetDamage() => damage;
    public float GetSpeed() => speed;
    public float GetAcceleration() => acceleration;
    public float GetAngularSpeed() => angularSpeed;
    public bool GetAutoBraking() => autoBraking;

}
