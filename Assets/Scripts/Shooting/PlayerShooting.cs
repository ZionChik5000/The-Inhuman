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
    [SerializeField] private LineRenderer lineRenderer; // —сылка на LineRenderer
    [SerializeField] private float lineWidth = 0.1f; // Ўирина линии трассировки

    [Header("Recoil Settings")]
    [SerializeField] private float recoilAngle = 2f;
    [SerializeField] private float recoilDuration = 0.1f;
    [SerializeField] private float recoilReturnDuration = 0.9f;
    [SerializeField] private float maxRecoilAngle = 10f;

    private bool isRecoiling = false;
    private float currentRecoilAngle = 0f;
    private Quaternion initialRotation;  // Store the initial rotation before the shot
    private Quaternion currentRotation;  // To keep track of current camera rotation
    private float currentVerticalAngle; // To track the vertical angle during recoil

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

        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer is not assigned in PlayerShooting script.");
        }
        else
        {
            lineRenderer.enabled = false; // Ensure LineRenderer is initially disabled
            lineRenderer.widthMultiplier = lineWidth; // ”станавливаем ширину линии

            // Assign the material to the LineRenderer (if it's not already assigned in the editor)
            if (lineRenderer.material == null)
            {
                lineRenderer.material = new Material(Shader.Find("Unlit/Transparent"));
                lineRenderer.material.color = new Color(1f, 1f, 1f, 1f); // White color with full opacity
            }
        }
    }

    private void Update()
    {
        // Check for the shooting input
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

        // Apply recoil effect (only if the recoil is happening)
        if (isRecoiling)
        {
            ApplyRecoil();
        }
    }

    private void Shoot()
    {
        if (fpsCam == null)
        {
            Debug.LogWarning("FPS Camera is not assigned, cannot shoot.");
            return;
        }

        // Start the LineRenderer to visualize the shot
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, fpsCam.transform.position); // Start point is the camera position

        // Perform the raycast to detect hits
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out RaycastHit hit, range, enemyLayer))
        {
            Debug.Log($"Hit: {hit.transform.name}");
            ProcessShot(hit);

            // End point is where the ray hits
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            // If no hit, the end point is at the max range
            lineRenderer.SetPosition(1, fpsCam.transform.position + fpsCam.transform.forward * range);
        }

        // Apply recoil effect
        StartRecoil();

        // Disable LineRenderer after a short time to simulate shot trail
        StartCoroutine(FadeLineRenderer());
    }

    private void ProcessShot(RaycastHit hit)
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

    private float CalculateDamage()
    {
        // Apply 1.5x damage if the player is in the air
        bool isAirborne = !playerMovement.IsGrounded() || playerMovement.GetState() == PlayerMovementAdvanced.MovementState.Air;
        return isAirborne ? damage * 1.5f : damage;
    }

    private void StartRecoil()
    {
        // Store the initial camera rotation
        initialRotation = fpsCam.transform.localRotation;
        currentRotation = initialRotation;

        // Increment the recoil angle (up to the maximum)
        currentRecoilAngle = Mathf.Min(currentRecoilAngle + recoilAngle, maxRecoilAngle);

        // Start the recoil coroutine
        isRecoiling = true;
        StartCoroutine(RecoilCoroutine());
    }

    private void ApplyRecoil()
    {
        // Apply the recoil effect based on the current vertical angle
        currentRotation = Quaternion.Euler(currentVerticalAngle - currentRecoilAngle, fpsCam.transform.localRotation.eulerAngles.y, fpsCam.transform.localRotation.eulerAngles.z);
        fpsCam.transform.localRotation = currentRotation;
    }

    private IEnumerator RecoilCoroutine()
    {
        float elapsedTime = 0f;
        currentVerticalAngle = fpsCam.transform.localRotation.eulerAngles.x;

        // Apply the recoil effect over time (only the vertical axis)
        while (elapsedTime < recoilDuration)
        {
            elapsedTime += Time.deltaTime;
            // Apply recoil only on the pitch (vertical angle)
            float verticalAngle = Mathf.Lerp(currentVerticalAngle, currentVerticalAngle - currentRecoilAngle, elapsedTime / recoilDuration);
            currentRotation = Quaternion.Euler(verticalAngle, fpsCam.transform.localRotation.eulerAngles.y, fpsCam.transform.localRotation.eulerAngles.z);
            fpsCam.transform.localRotation = currentRotation;
            yield return null;
        }

        // After recoil, start returning to the original rotation
        StartCoroutine(ReturnToOriginalRotation());
    }

    private IEnumerator ReturnToOriginalRotation()
    {
        float elapsedTime = 0f;
        Quaternion targetRotation = Quaternion.Euler(initialRotation.eulerAngles.x, currentRotation.eulerAngles.y, currentRotation.eulerAngles.z);

        // Gradually return to the initial rotation, only affecting vertical (pitch) axis
        while (elapsedTime < recoilReturnDuration)
        {
            elapsedTime += Time.deltaTime;
            // Smooth return to the vertical (pitch) component of rotation, but not horizontal (yaw)
            currentRotation = Quaternion.Euler(
                Mathf.LerpAngle(currentRotation.eulerAngles.x, initialRotation.eulerAngles.x, elapsedTime / recoilReturnDuration),
                currentRotation.eulerAngles.y, currentRotation.eulerAngles.z);
            fpsCam.transform.localRotation = currentRotation;
            yield return null;
        }

        // Ensure the camera is fully returned to its initial state
        fpsCam.transform.localRotation = Quaternion.Euler(initialRotation.eulerAngles.x, fpsCam.transform.localRotation.eulerAngles.y, fpsCam.transform.localRotation.eulerAngles.z);
        isRecoiling = false;
    }

    // Coroutine to fade out and disable the LineRenderer
    private IEnumerator FadeLineRenderer()
    {
        float fadeDuration = 1f; // Duration of the fade effect
        float elapsedTime = 0f;
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        Gradient gradient = lineRenderer.colorGradient;

        // Capture the initial color gradient
        GradientAlphaKey[] initialAlphaKeys = gradient.alphaKeys;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            // Update the alpha keys
            for (int i = 0; i < alphaKeys.Length; i++)
            {
                alphaKeys[i] = new GradientAlphaKey(alpha, initialAlphaKeys[i].time);
            }

            gradient.SetKeys(gradient.colorKeys, alphaKeys);
            lineRenderer.colorGradient = gradient;

            yield return null;
        }

        // Ensure the LineRenderer is fully invisible and disable it
        lineRenderer.enabled = false;
    }
}
