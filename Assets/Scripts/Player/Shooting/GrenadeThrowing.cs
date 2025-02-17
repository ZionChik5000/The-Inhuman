using System.Collections;
using UnityEngine;

public class GrenadeWeapon : WeaponBase
{
    [Header("Grenade Settings")]
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionDamage = 50f;
    [SerializeField] private float fuseTime = 3f;

    private GameObject heldGrenade;

    public Transform handPosition;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void SpawnHeldGrenade()
    {
        if (heldGrenade == null)
        {
            heldGrenade = Instantiate(grenadePrefab, handPosition.position, handPosition.rotation, handPosition);
            Destroy(heldGrenade.GetComponent<Rigidbody>());
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

        Destroy(heldGrenade);
        StartCoroutine(ExplodeAfterDelay(thrownGrenade));
        SpawnHeldGrenade();
    }

    private IEnumerator ExplodeAfterDelay(GameObject grenade)
    {
        yield return new WaitForSeconds(fuseTime);
        Explode(grenade);
    }

    private void Explode(GameObject grenade)
    {
        Collider[] hitColliders = Physics.OverlapSphere(grenade.transform.position, explosionRadius);

        foreach (Collider hit in hitColliders)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (hit.TryGetComponent(out Enemy enemy))
                {
                    enemy.TakeDamage(explosionDamage);
                }
            }
        }

        Destroy(grenade);
    }

    public void Initialize(Transform handPosition)
    {
        this.handPosition = handPosition;
    }
}
