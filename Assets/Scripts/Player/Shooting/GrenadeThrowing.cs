using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Throw Settings")]
    public float throwForce = 15f; 
    public float upForce = 5f;     

    [Header("Explosion Settings")]
    public float explosionDelay = 3f; 
    public float explosionRadius = 5f; 
    public float explosionForce = 700f; 
    public GameObject explosionEffect; 

    private Rigidbody rb;
    private bool hasExploded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Vector3 throwDirection = Camera.main.transform.forward + Vector3.up * upForce;
        rb.AddForce(throwDirection.normalized * throwForce, ForceMode.Impulse);

        StartCoroutine(ExplosionCountdown());
    }

    private IEnumerator ExplosionCountdown()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }

    private void Explode()
    {

        if (hasExploded) return;
        hasExploded = true;

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            // Применяем взрывную силу к объектам с Rigidbody
            Rigidbody nearbyRb = nearbyObject.GetComponent<Rigidbody>();
            if (nearbyRb != null)
            {
                nearbyRb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            // Наносим урон, если объект имеет компонент здоровья
            var health = nearbyObject.GetComponent<Enemy>();
            if (health != null)
            {
                health.TakeDamage(50); // Урон от гранаты
            }
        }

        // Уничтожаем гранату
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Отображаем радиус взрыва в редакторе
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
