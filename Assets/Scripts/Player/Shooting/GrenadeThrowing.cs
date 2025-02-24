using System.Collections;
using UnityEngine;

public class GrenadeWeapon : WeaponBase
{
    [Header("Grenade Settings")]
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private float throwForce = 500f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionDamage = 50f;
    [SerializeField] private float fuseTime = 3f;

    [SerializeField] private GameObject heldGrenade;
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
            heldGrenade = Instantiate(heldGrenade, _handPosition.position, _handPosition.rotation, _handPosition);
        }
    }

    public override async void Shoot()
    { 
        if (heldGrenade == null) Debug.Log("Gooool");

        GameObject thrownGrenade = Instantiate(grenadePrefab, heldGrenade.transform.position, heldGrenade.transform.rotation);
        Rigidbody rb = thrownGrenade.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(fpsCam.transform.forward * throwForce, ForceMode.VelocityChange);
        }

        Destroy(heldGrenade);
        await StartCoroutine(Delay());
        await Explode(thrownGrenade);
        await SpawnHeldGrenade();
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(fuseTime);
    }

    private void Explode(GameObject grenade)
    {
        Debug.Log("Goida");

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
}
