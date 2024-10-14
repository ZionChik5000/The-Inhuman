using System;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public Camera fpsCam;
    public LayerMask enemyLayer;
    public PlayerMovementAdvanced playerMovement; // Reference to the PlayerMovement script


    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
            
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range, enemyLayer))
        {
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                float finalDamage = damage;
                if (!playerMovement.grounded) // Check if the player is not grounded (jumping or sliding)
                {
                    finalDamage *= 1.5f;
                }

                enemy.TakeDamage(finalDamage);
            }
        }
    }
}
