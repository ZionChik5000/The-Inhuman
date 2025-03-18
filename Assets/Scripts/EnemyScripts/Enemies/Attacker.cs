using UnityEngine;

public class Attacker : EnemyMod
{
    private float lastAttackTime = 0f;
    private Enemy enemy;

    public override void Apply(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public override void ModUpdate(Enemy enemy)
    {
        enemy.pathUpdateTimer += Time.deltaTime;

        if (enemy.playerInRadius && enemy.player != null && enemy.pathUpdateTimer >= enemy.pathUpdateInterval)
        {
            enemy.agent.SetDestination(enemy.player.position);
            enemy.pathUpdateTimer = 0f;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (enemy == null) return;

        if (collision.collider.CompareTag("Player") && Time.time >= lastAttackTime + enemy.GetAttackCooldownS())
        {
            HpController playerHp = collision.collider.GetComponentInParent<HpController>()
                                    ?? collision.collider.GetComponentInChildren<HpController>()
                                    ?? FindObjectOfType<HpController>();

            if (playerHp != null)
            {
                playerHp.TakeDamage(enemy.GetDamage());
                lastAttackTime = Time.time;
            }
            else
            {
                Debug.LogError("HpController not found!");
            }
        }
    }
}
