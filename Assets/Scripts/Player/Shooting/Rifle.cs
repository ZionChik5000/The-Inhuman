using System.Collections;
using UnityEngine;

public class Rifle : WeaponBase
{
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float maxSpreadAngle = 15f;
    [SerializeField] private float recoilShakeIntensity = 0.1f;
    [SerializeField] private float recoilShakeDuration = 0.1f;

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

        StartCoroutine(ShakeCamera());
    }

    private Vector3 ApplySpread(Vector3 direction)
    {
        float randomYaw = Random.Range(-maxSpreadAngle, maxSpreadAngle);
        float randomPitch = Random.Range(-maxSpreadAngle, maxSpreadAngle);

        Quaternion spreadRotation = Quaternion.Euler(randomPitch, randomYaw, 0);
        return spreadRotation * direction;
    }

    private IEnumerator ShakeCamera()
    {
        Vector3 originalPosition = fpsCam.transform.localPosition;
        float elapsedTime = 0;

        while (elapsedTime < recoilShakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * recoilShakeIntensity;
            float offsetY = Random.Range(-1f, 1f) * recoilShakeIntensity;

            fpsCam.transform.localPosition = new Vector3(originalPosition.x + offsetX, originalPosition.y + offsetY, originalPosition.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fpsCam.transform.localPosition = originalPosition;
    }
}
