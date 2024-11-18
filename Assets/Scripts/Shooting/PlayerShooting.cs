using System;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 100f;
    [SerializeField] private Camera fpsCam;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private PlayerMovementAdvanced playerMovement;

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out RaycastHit hit, range, enemyLayer))
        {
            ProcessShot(hit);
        }
    }

    private void ProcessShot(RaycastHit hit)
    {
        Enemy enemy = hit.transform.GetComponent<Enemy>();
        if (enemy != null)
        {
            float finalDamage = CalculateDamage();
            enemy.TakeDamage(finalDamage);
        }
    }

    private float CalculateDamage()
    {
        return (!playerMovement.IsGrounded() || playerMovement.GetState() == PlayerMovementAdvanced.MovementState.Air) ? damage * 1.5f : damage;
    }
}
