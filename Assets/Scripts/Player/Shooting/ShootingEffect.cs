using UnityEngine;

public class ShootingEffect : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Camera fpsCam;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private float lineWidth = 0.1f;

    private float alpha = 0f;
    private bool fading = false;

    private void Awake()
    {
        if (lineRenderer != null)
        {
            lineRenderer.useWorldSpace = true;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Shoot();

        if (fading)
            FadeOut();
    }

    private void Shoot()
    {
        if (lineRenderer == null || fpsCam == null)
            return;

        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 startPos = ray.origin;
        Vector3 endPos = ray.origin + ray.direction * range;

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
        alpha = 1f;
        fading = true;
        lineRenderer.enabled = true;
    }

    private void FadeOut()
    {
        if (alpha <= 0f)
        {
            lineRenderer.enabled = false;
            fading = false;
            return;
        }

        alpha -= Time.deltaTime / fadeSpeed;

        Color currentColor = lineRenderer.startColor;
        Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, Mathf.Clamp01(alpha));
        lineRenderer.startColor = newColor;
        lineRenderer.endColor = newColor;
    }
}
