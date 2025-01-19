using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] Transform[] waypoints;
    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;


    [Header("Congifuration")]
    public float health = 50f;
    public float damage = 10f;

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider)
    {
        if (Collider.gameObject.tag == "Player")
        {
            
        }
    }
}
