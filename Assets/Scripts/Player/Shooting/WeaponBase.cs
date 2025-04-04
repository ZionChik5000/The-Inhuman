using System;
using System.Collections;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private float damage = 10f;
    [SerializeField] protected float range = 500f;
    [SerializeField] protected LayerMask enemyLayer;

    protected Camera fpsCam;
    private PlayerMovementAdvanced playerMovement;
    protected LineRenderer lineRenderer;
    protected Transform _handPosition;

    [SerializeField] private float lineWidth = 0.1f;


    public abstract void Shoot();

    public void ProcessShot(RaycastHit hit)
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

    public float CalculateDamage()
    {
        bool isAirborne = !playerMovement.IsGrounded() || playerMovement.GetState() == PlayerMovementAdvanced.MovementState.Air;
        return isAirborne ? damage * 1.5f : damage;
    }

    public IEnumerator FadeLineRenderer()
    {
        float fadeDuration = 0.2f;
        float elapsedTime = 0f;
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        Gradient gradient = lineRenderer.colorGradient;

        GradientAlphaKey[] initialAlphaKeys = gradient.alphaKeys;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            for (int i = 0; i < alphaKeys.Length; i++)
            {
                alphaKeys[i] = new GradientAlphaKey(alpha, initialAlphaKeys[i].time);
            }

            gradient.SetKeys(gradient.colorKeys, alphaKeys);
            lineRenderer.colorGradient = gradient;

            yield return null;
        }

        lineRenderer.enabled = false;
    }

    public void Initialize(Camera fpsCam, LineRenderer lineRenderer, PlayerMovementAdvanced playerMovement, Transform handPosition)
    {
        this.fpsCam = fpsCam;
        this.lineRenderer = lineRenderer;
        this.playerMovement = playerMovement;
        _handPosition = handPosition;

        if (fpsCam == null)
        {
            Debug.LogError("FPS Camera is not assigned in WeaponBase script.");
        }

        if (playerMovement == null)
        {
            Debug.LogError("Player Movement component is not assigned in PlayerShooting script.");
        }

        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer is not assigned in PlayerShooting script.");
        }
        else
        {
            lineRenderer.enabled = false;
            lineRenderer.widthMultiplier = lineWidth;

            if (lineRenderer.material == null)
            {
                lineRenderer.material = new Material(Shader.Find("Unlit/Transparent"));
                lineRenderer.material.color = new Color(1f, 1f, 1f, 1f);
            }
        }
    }
}
