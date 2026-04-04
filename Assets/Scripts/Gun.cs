using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    public float range = 100f;
    public int damage = 50;

    public Camera fpsCamera;
    public LineRenderer lineRenderer;

    [Header("Efekty")]
    public GameObject bloodDecal;   // 🩸 plama krwi na ciele
    public GameObject deathBloodEffect;

    [Header("Dźwięk")]
    public AudioSource audioSource;
    public AudioClip headshotSound;

    void Start()
    {
        if (fpsCamera == null)
            fpsCamera = GetComponentInParent<Camera>();

        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Shoot();
    }

    void Shoot()
    {
        RaycastHit hit;

        Vector3 origin = fpsCamera.transform.position;
        Vector3 direction = fpsCamera.transform.forward;

        Vector3 endPoint = origin + direction * range;

        if (Physics.Raycast(origin, direction, out hit, range))
        {
            endPoint = hit.point;

            EnemyHealth enemy = hit.collider.GetComponentInParent<EnemyHealth>();

            if (enemy != null)
            {
                bool isHead = hit.collider.CompareTag("Head");

                enemy.TakeDamage(isHead ? damage * 2 : damage);

                // 🎯 HEADSHOT
                if (isHead)
                {
                    if (audioSource != null && headshotSound != null)
                        audioSource.PlayOneShot(headshotSound);
                }
                else
                {
                    // 🩸 BLOOD DECAL TYLKO NA CIAŁO
                    SpawnBloodDecal(hit);
                }

                // 💀 KREW PRZY ŚMIERCI
                if (enemy.IsDead())
                {
                    SpawnDeathBlood(hit.point);
                }
            }
        }

        if (lineRenderer != null)
        {
            StartCoroutine(ShowLaser(origin, endPoint));
        }
    }

    void SpawnBloodDecal(RaycastHit hit)
    {
        if (bloodDecal == null) return;

        Quaternion rot = Quaternion.LookRotation(hit.normal);

        Vector3 pos = hit.point + hit.normal * 0.01f; // żeby nie wchodziło w model

        GameObject decal = Instantiate(bloodDecal, pos, rot);

        Destroy(decal, 10f); // usuwa po czasie
    }

    void SpawnDeathBlood(Vector3 pos)
    {
        if (deathBloodEffect != null)
        {
            GameObject fx = Instantiate(deathBloodEffect, pos, Quaternion.identity);
            Destroy(fx, 3f);
        }
    }

    IEnumerator ShowLaser(Vector3 start, Vector3 end)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        yield return new WaitForSeconds(0.05f);

        lineRenderer.enabled = false;
    }
}