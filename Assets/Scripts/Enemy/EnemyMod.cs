using UnityEngine;

public abstract class EnemyMod : MonoBehaviour
{
    public abstract void Apply(Enemy enemy);
    public abstract void ModUpdate(Enemy enemy);
}


/*
----- MOD EXAMPLE -----

using UnityEngine;

public class SpeedBoostMod : EnemyMod
{
    public override void Apply(Enemy enemy)
    {
        enemy.SetSpeed(enemy.GetSpeed() * 2);
        Debug.Log("SpeedBoost applied!");
    }

    public override void ModUpdate(Enemy enemy)
    {
        Debug.Log("SpeedBoost active!");
    }
}

Don't forget to add: 
public class MODNAME : EnemyMod
*/
