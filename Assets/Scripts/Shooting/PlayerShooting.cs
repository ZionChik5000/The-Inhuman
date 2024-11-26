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

    private void Awake()
    {
        if (fpsCam == null)
        {
            Debug.LogWarning("FPS Camera is not assigned in PlayerShooting script.");
        }

        if (playerMovement == null)
        {
            Debug.LogWarning("Player Movement component is not assigned in PlayerShooting script.");
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (fpsCam == null)
        {
            Debug.LogWarning("FPS Camera is not assigned, cannot shoot.");
            return;
        }

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out RaycastHit hit, range, enemyLayer))
        {
            Debug.Log($"Hit: {hit.transform.name}");
            ProcessShot(hit);
        }
        else
        {
            Debug.Log("No hit detected.");
        }
    }

    private void ProcessShot(RaycastHit hit)
    {
        Enemy enemy = hit.transform.GetComponent<Enemy>();
        if (enemy != null)
        {
            Debug.Log("Enemy hit, processing damage.");
            float finalDamage = CalculateDamage();
            enemy.TakeDamage(finalDamage);
        }
        else
        {
            Debug.Log("Hit object is not an enemy.");
        }
    }

    private float CalculateDamage()
    {
        return (!playerMovement.IsGrounded() || playerMovement.GetState() == PlayerMovementAdvanced.MovementState.Air) ? damage * 1.5f : damage;
    }
}
