using System.Collections;
using UnityEngine;

public class Rifle : WeaponBase
{
    [SerializeField] private float fireRate = 0.2f; 
    [SerializeField] private float maxSpreadAngle = 8f; 
    private bool isShooting;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isShooting = true;
            StartCoroutine(AutoFire());
        }

        if (Input.GetMouseButtonUp(0))
        {
            isShooting = false;
        }
    }

    private IEnumerator AutoFire()
    {
        while (isShooting)
        {
            Shoot();
            yield return new WaitForSeconds(fireRate);
        }
    }

    public override void Shoot()
    {
        if (fpsCam == null)
        {
            Debug.LogWarning("FPS Camera is not assigned, cannot shoot.");
            return;
        }

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, fpsCam.transform.position);

        Vector3 direction = fpsCam.transform.forward;
        direction = ApplySpread(direction);

        if (Physics.Raycast(fpsCam.transform.position, direction, out RaycastHit hit, range, enemyLayer))
        {
            Debug.Log($"Hit: {hit.transform.name}");
            ProcessShot(hit);
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.SetPosition(1, fpsCam.transform.position + direction * range);
        }

        StartCoroutine(FadeLineRenderer());
    }

    private Vector3 ApplySpread(Vector3 direction)
    {
        float randomYaw = Random.Range(-maxSpreadAngle, maxSpreadAngle);
        float randomPitch = Random.Range(-maxSpreadAngle, maxSpreadAngle);

        Quaternion spreadRotation = Quaternion.Euler(randomPitch, randomYaw, 0);
        return spreadRotation * direction;
    }
}
