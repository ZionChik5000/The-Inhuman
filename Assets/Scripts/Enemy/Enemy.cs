using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float health = 50f;

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
