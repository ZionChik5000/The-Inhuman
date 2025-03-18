using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    public float attackCooldown = 3f;
    public GameObject projectilePrefab;
    public int projectileCount = 6;
    public float projectileSpeed = 5f;
    public Color dashColor = Color.red;
    public int damage = 20;

    private Transform player;
    private Color originalColor;
    private Renderer bossRenderer;
    private Rigidbody rb;
    private bool isDashing = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        bossRenderer = GetComponent<Renderer>();
        originalColor = bossRenderer.material.color;
        rb = GetComponent<Rigidbody>();
        StartCoroutine(AttackCycle());
    }

    void Update()
    {
        if (!isDashing)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
        }
    }

    IEnumerator AttackCycle()
    {
        while (true)
        {
            ShootProjectiles();
            yield return new WaitForSeconds(attackCooldown);
            yield return StartCoroutine(DashAttack());
            yield return new WaitForSeconds(attackCooldown);
        }
    }

    void ShootProjectiles()
    {
        float angleStep = 360f / projectileCount;
        float spawnDistance = 3f; // –ассто€ние дл€ создани€ снар€дов вне босса
        for (int i = 0; i < projectileCount; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            Vector3 spawnPosition = transform.position + direction * spawnDistance;
            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            projectile.GetComponent<Projectile>().Initialize(direction, projectileSpeed, damage);
        }
    }

    IEnumerator DashAttack()
    {
        rb.velocity = Vector3.zero;
        bossRenderer.material.color = dashColor;
        yield return new WaitForSeconds(1f);

        Vector3 dashDirection = (player.position - transform.position).normalized;
        rb.velocity = dashDirection * dashSpeed;
        isDashing = true;
        yield return new WaitForSeconds(dashDuration);

        rb.velocity = Vector3.zero;
        bossRenderer.material.color = originalColor;
        isDashing = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HpController playerHealth = collision.gameObject.GetComponent<HpController>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
