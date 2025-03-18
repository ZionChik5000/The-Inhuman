using UnityEngine;

public class Shooter : EnemyMod
{
    private Enemy enemy;
    [SerializeField] private float retreatDistance = 5f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float shootDelay = 1f;
    private float lastShootTime;

    public override void Apply(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public override void ModUpdate(Enemy enemy)
    {
        Shoot();
        Retret();
    }

    private float DistanceToPlayer()
    {
        return Vector3.Distance(enemy.transform.position, enemy.player.position);
    }

    private void Retret()
    {
        if (!enemy.playerInRadius || enemy.player == null) return;

        if (DistanceToPlayer() < retreatDistance)
        {
            Vector3 retreatDirection = (enemy.transform.position - enemy.player.position).normalized;
            retreatDirection.y = 0;
            Vector3 targetPosition = enemy.player.position + retreatDirection * retreatDistance;

            enemy.agent.SetDestination(targetPosition);
        }
    }

    private void Shoot()
    {
        if (Time.time - lastShootTime < shootDelay || bulletPrefab == null || shootPoint == null) return;

        if (Physics.Raycast(enemy.transform.position, enemy.transform.forward, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
                lastShootTime = Time.time;
            }
        }
    }
}
