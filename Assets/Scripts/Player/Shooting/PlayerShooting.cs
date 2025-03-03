using System.Collections;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 100f;
    [SerializeField] private Camera fpsCam;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private PlayerMovementAdvanced playerMovement;

    [Header("Recoil Settings")]
    [SerializeField] private float recoilAngle = 2f;
    [SerializeField] private float recoilDuration = 0.1f;
    [SerializeField] private float recoilReturnDuration = 0.9f;
    [SerializeField] private float maxRecoilAngle = 10f;

    private bool isRecoiling = false;
    private float currentRecoilAngle = 0f;

    private void Awake()
    {
        if (fpsCam == null)
        {
            Debug.LogError("FPS Camera is not assigned in PlayerShooting script.");
        }

        if (playerMovement == null)
        {
            Debug.LogError("Player Movement component is not assigned in PlayerShooting script.");
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isRecoiling)
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

        RaycastHit[] hits = Physics.RaycastAll(fpsCam.transform.position, fpsCam.transform.forward, range, enemyLayer);

        foreach (RaycastHit hit in hits)
        {
            if (!hit.collider.isTrigger)
            {
                ProcessShot(hit);
                break;
            }
        }

        StartRecoil();
    }

    private void ProcessShot(RaycastHit hit)
    {
        Enemy enemy = hit.transform.GetComponent<Enemy>();
        if (enemy != null)
        {
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
        bool isAirborne = !playerMovement.IsGrounded() || playerMovement.GetState() == PlayerMovementAdvanced.MovementState.Air;
        return isAirborne ? damage * 1.5f : damage;
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
