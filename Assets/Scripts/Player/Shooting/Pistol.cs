using System.Collections;
using UnityEngine;

public class Pistol : WeaponBase
{

    [Header("Recoil Settings")]
    [SerializeField] private float recoilAngle = 2f;
    [SerializeField] private float recoilDuration = 0.1f;
    [SerializeField] private float recoilReturnDuration = 0.9f;
    [SerializeField] private float maxRecoilAngle = 10f;

    private bool isRecoiling = false;
    private float currentRecoilAngle = 0f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isRecoiling)
        {
            Shoot();
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

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out RaycastHit hit, range, enemyLayer))
        {
            Debug.Log($"Hit: {hit.transform.name}");
            ProcessShot(hit);
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.SetPosition(1, fpsCam.transform.position + fpsCam.transform.forward * range);
        }

        StartRecoil();
        StartCoroutine(FadeLineRenderer());
    }

    private void StartRecoil()
    {
        currentRecoilAngle = Mathf.Min(currentRecoilAngle + recoilAngle, maxRecoilAngle);
        isRecoiling = true;
        StartCoroutine(RecoilCoroutine());
    }

    private IEnumerator RecoilCoroutine()
    {
        float elapsedTime = 0f;
        float recoilOffset = 0f;

        while (elapsedTime < recoilDuration)
        {
            elapsedTime += Time.deltaTime;

            float targetOffset = Mathf.Lerp(0f, currentRecoilAngle, elapsedTime / recoilDuration);
            float delta = targetOffset - recoilOffset;

            fpsCam.transform.localRotation *= Quaternion.Euler(-delta, 0f, 0f);

            recoilOffset = targetOffset;

            yield return null;
        }

        StartCoroutine(ReturnToOriginalRotation(recoilOffset));
    }




    private IEnumerator ReturnToOriginalRotation(float recoilOffset)
    {
        float elapsedTime = 0f;

        while (elapsedTime < recoilReturnDuration)
        {
            elapsedTime += Time.deltaTime;

            float targetOffset = Mathf.Lerp(recoilOffset, 0f, elapsedTime / recoilReturnDuration);
            float delta = recoilOffset - targetOffset;

            fpsCam.transform.localRotation *= Quaternion.Euler(delta, 0f, 0f);

            recoilOffset = targetOffset;

            yield return null;
        }

        isRecoiling = false;
    }
}
