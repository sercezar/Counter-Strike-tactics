using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    public float range = 100f;       // Zasięg strzału
    public int damage = 50;          // Obrażenia
    public float force = 500f;       // Siła uderzenia

    public Camera fpsCamera;         // Kamera FPS
    public LineRenderer lineRenderer; // LineRenderer lasera

    void Start()
    {
        if (fpsCamera == null)
            fpsCamera = GetComponentInParent<Camera>();

        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;

            // 🔴 Ustawienie koloru lasera na czerwony
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
        }
    }

    void Update()
    {
        // Lewy przycisk myszy
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit hit;

        Vector3 origin = fpsCamera.transform.position;
        Vector3 direction = fpsCamera.transform.forward;
        Vector3 endPoint;

        if (Physics.Raycast(origin, direction, out hit, range))
        {
            Debug.Log("Trafiono: " + hit.transform.name);
            endPoint = hit.point;

            // 💥 OBRAŻENIA
            EnemyHealth enemy = hit.collider.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // 💥 SIŁA UDERZENIA
            Rigidbody rb = hit.collider.GetComponentInParent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForceAtPosition(direction * force, hit.point);
            }
        }
        else
        {
            endPoint = origin + direction * range;
        }

        // 🔴 WYŚWIETLENIE LASERA
        if (lineRenderer != null)
        {
            StartCoroutine(ShowLaser(origin, endPoint));
        }
    }

    IEnumerator ShowLaser(Vector3 start, Vector3 end)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        yield return new WaitForSeconds(0.05f); // laser świeci krótko

        lineRenderer.enabled = false;
    }
}