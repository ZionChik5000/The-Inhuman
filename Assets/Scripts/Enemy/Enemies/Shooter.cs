using UnityEngine;
using UnityEngine.AI;

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
        if (enemy.playerInRadius && enemy.player != null)
        {
            HandleRetreat();
            HandleShooting();

            //Debug raycast
            if (shootPoint != null && enemy?.player != null)
            {
                Vector3 directionToPlayer = (enemy.player.position - shootPoint.position).normalized;
                Debug.DrawRay(shootPoint.position, directionToPlayer * 100, Color.red);
            }
        }
    }

    private float DistanceToPlayer()
    {
        if (enemy.player == null)
            return Mathf.Infinity;
        return Vector3.Distance(enemy.transform.position, enemy.player.position);
    }

    private void HandleRetreat()
    {
        if (DistanceToPlayer() < retreatDistance)
        {
            Vector3 retreatDirection = (enemy.transform.position - enemy.player.position).normalized;
            retreatDirection.y = 0;
            Vector3 targetPosition = enemy.player.position + retreatDirection * retreatDistance;
            enemy.agent.SetDestination(targetPosition);
        }
    }

    private void HandleShooting()
    {
        if (Time.time - lastShootTime < shootDelay || bulletPrefab == null || shootPoint == null)
            return;

        Vector3 directionToPlayer = (enemy.player.position - shootPoint.position).normalized;

        if (Physics.Raycast(shootPoint.position, directionToPlayer, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Instantiate(bulletPrefab, shootPoint.position, Quaternion.LookRotation(directionToPlayer));
                lastShootTime = Time.time;
            }
        }
    }
}
