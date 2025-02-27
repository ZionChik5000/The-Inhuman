using System.Collections;
using UnityEngine;

public class GrenadeWeapon : WeaponBase
{
    [Header("Grenade Settings")]
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private float throwForce = 500f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionDamage = 50f;
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeMagnitude = 0.3f;

    private float fuseTime = 3f;

    [SerializeField] private GameObject heldGrenade;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && heldGrenade.activeInHierarchy)
        {

            Shoot();
        }
    }

    public override void Shoot()
    { 
        if (heldGrenade == null) Debug.Log("Gooool");

        GameObject thrownGrenade = Instantiate(grenadePrefab, heldGrenade.transform.position, heldGrenade.transform.rotation);
        Rigidbody rb = thrownGrenade.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(fpsCam.transform.forward * throwForce, ForceMode.VelocityChange);
        }

        heldGrenade.SetActive(false);
        StartCoroutine(ThrowingGrenade(thrownGrenade));
        
    }

    private IEnumerator ThrowingGrenade(GameObject grenade)
    {
        Debug.Log("Throwing Grenade");
        Debug.Log(fuseTime);
        yield return new WaitForSeconds(fuseTime);
        Debug.Log("After yield");

        Explode(grenade);
        
    }

    private void Explode(GameObject grenade)
    {
        Debug.Log("Explode");

        StartCoroutine(CameraShake());

        int enemyLayer = LayerMask.NameToLayer("Enemy");
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        GameObject explosionEffect = Instantiate(explosionEffectPrefab, grenade.transform.position, Quaternion.identity);

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == enemyLayer)
            {
                float distance = Vector3.Distance(grenade.transform.position, obj.transform.position);
                if (distance <= 8f)
                {
                    if (obj.TryGetComponent(out Enemy enemy))
                    {
                        enemy.TakeDamage(explosionDamage);
                    }
                }
            }
        }

        Destroy(grenade);
        Destroy(explosionEffect, 2f);
        heldGrenade.SetActive(true);
    }

    private IEnumerator CameraShake()
    {
        Vector3 originalPosition = fpsCam.transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            fpsCam.transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fpsCam.transform.localPosition = originalPosition;
    }

    private void OnDestroy()
    {
        Debug.Log("On Destroy");
    }
}
