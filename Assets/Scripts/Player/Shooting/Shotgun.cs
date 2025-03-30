using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : WeaponBase
{
    [Header("Shotgun Settings")]
    [SerializeField] private int pellets = 12; 
    [SerializeField] private float spreadAngle = 7f; 
    [SerializeField] private float recoilAngle = 15f; 
    [SerializeField] private float recoilDuration = 0.1f; 
    [SerializeField] private float recoilReturnDuration = 0.9f; 
    [SerializeField] private float maxRecoilAngle = 15f; 
    [SerializeField] private GameObject lineRendererPrefab; 
    [SerializeField] private int poolSize = 20; 
    [SerializeField] private float fireRate = 2f;

    private bool isRecoiling = false;
    private bool isShooting = false;
    private float currentRecoilAngle = 0f;
    public Transform Marmo3;

    private Queue<GameObject> lineRendererPool;

    private void Start()
    {
        lineRendererPool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject line = Instantiate(lineRendererPrefab, Marmo3);
            line.SetActive(false);
            lineRendererPool.Enqueue(line);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isRecoiling)
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
        if (weaponCamera == null)
        {
            Debug.LogWarning("FPS Camera is not assigned, cannot shoot.");
            return;
        }

        SoundManager.Play("shotgun");

        for (int i = 0; i < pellets; i++)
        {
            Vector3 spread = GetRandomSpreadDirection();
            if (Physics.Raycast(weaponCamera.transform.position, spread, out RaycastHit hit, range, enemyLayer))
            {
                ProcessShot(hit);
                ShowTracer(weaponCamera.transform.position, hit.point);
            }
            else
            {
                Vector3 endPoint = weaponCamera.transform.position + spread * range;
                ShowTracer(weaponCamera.transform.position, endPoint);
            }
        }

        StartRecoil();
    }

    private Vector3 GetRandomSpreadDirection()
    {
        float randomYaw = Random.Range(-spreadAngle, spreadAngle);
        float randomPitch = Random.Range(-spreadAngle, spreadAngle);
        Quaternion spreadRotation = Quaternion.Euler(randomPitch, randomYaw, 0);
        return spreadRotation * weaponCamera.transform.forward;
    }

    private void ShowTracer(Vector3 start, Vector3 end)
    {
        if (lineRendererPool.Count > 0)
        {
            GameObject tracer = lineRendererPool.Dequeue();
            tracer.SetActive(true);
            LineRenderer lr = tracer.GetComponent<LineRenderer>();

            lr.SetPosition(0, start);
            lr.SetPosition(1, end);

            StartCoroutine(FadeAndReturnTracer(tracer));
        }
    }

    private IEnumerator FadeAndReturnTracer(GameObject tracer)
    {
        yield return new WaitForSeconds(0.2f);
        tracer.SetActive(false);
        lineRendererPool.Enqueue(tracer);
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
