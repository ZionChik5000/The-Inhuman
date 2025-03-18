using UnityEngine;

public abstract class EnemyMod : MonoBehaviour
{
    public abstract void Apply(Enemy enemy);
    public abstract void ModUpdate(Enemy enemy);
}



/*
----- MOD EXAMPLE -----

public class Attacker : EnemyMod
{
    private float lastAttackTime = 0f;
    private Enemy enemy;

    public override void Apply(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public override void ModUpdate(Enemy enemy) { }

    private void OnTriggerStay(Collider other)
    {
        if (enemy == null) return;

        if (other.CompareTag("Player") && Time.time >= lastAttackTime + enemy.GetAttackCooldownS())
        {
            HpController playerHp = other.GetComponentInParent<HpController>()
                                    ?? other.GetComponentInChildren<HpController>()
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

*/
