using UnityEngine;

public class Shooter : EnemyMod
{
    private Enemy enemy;
    [SerializeField] private float retreatDistance = 5f;

    public override void Apply(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public override void ModUpdate(Enemy enemy)
    {
        if (!enemy.playerInRadius || enemy.player == null) return;

        if (DistanceToPlayer() < retreatDistance)
        {
            Vector3 retreatDirection = (enemy.transform.position - enemy.player.position).normalized;
            Vector3 targetPosition = enemy.player.position + retreatDirection * retreatDistance;
            Vector3 moveDirection = (targetPosition - enemy.transform.position).normalized;
            enemy.agent.Move(moveDirection * enemy.speed * Time.deltaTime);
        }
    }

    private float DistanceToPlayer()
    {
        return Vector3.Distance(enemy.transform.position, enemy.player.position);
    }
}
