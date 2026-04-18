using UnityEngine;
using System.Collections;
using TMPro;

public class Gun : MonoBehaviour
{
    [Header("Strzelanie")]
    public float range = 100f;
    public int damage = 50;
    public float fireRate = 0.1f;
    public bool isAutomatic = true;

    [Header("Magazynek")]
    public int magazineSize = 30;
    public int currentAmmo;
    public float reloadTime = 1.5f;
    private bool isReloading = false;

    [Header("Dźwięki")]
    public AudioClip reloadSound;
    public AudioClip headshotSound;

    [Header("Krew")]
    public GameObject bloodHitEffect;
    public GameObject bloodDeathEffect;

    [Header("Animacja")]
    public Animator gunAnimator;

    [Header("UI")]
    public TextMeshProUGUI ammoText;

    public Camera fpsCamera;
    private AudioSource audioSource;

    float nextTimeToFire = 0f;

    void Start()
    {
        currentAmmo = magazineSize;

        if (fpsCamera == null)
            fpsCamera = GetComponentInParent<Camera>();

        audioSource = GetComponent<AudioSource>();

        UpdateAmmoUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < magazineSize)
        {
            StartCoroutine(Reload());
            return;
        }

        if (isReloading) return;

        if (isAutomatic ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0))
        {
            if (Time.time >= nextTimeToFire && currentAmmo > 0)
            {
                nextTimeToFire = Time.time + fireRate;

                Shoot();

                currentAmmo--;
                UpdateAmmoUI();
            }
        }
    }

    void Shoot()
    {
        RaycastHit hit;

        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range))
        {
            EnemyHealth enemy = hit.collider.GetComponentInParent<EnemyHealth>();

            if (enemy != null)
            {
                bool isHead = hit.collider.CompareTag("Head");

                enemy.TakeDamage(isHead ? damage * 2 : damage);

                if (isHead && headshotSound != null)
                    audioSource.PlayOneShot(headshotSound);

                if (bloodHitEffect != null)
                {
                    GameObject fx = Instantiate(bloodHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(fx, 3f);
                }

                if (enemy.currentHealth <= 0 && bloodDeathEffect != null)
                {
                    GameObject fx = Instantiate(bloodDeathEffect, hit.point, Quaternion.identity);
                    Destroy(fx, 3f);
                }
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;

        if (gunAnimator != null)
            gunAnimator.SetTrigger("Reload");

        if (reloadSound != null)
            audioSource.PlayOneShot(reloadSound);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;
        UpdateAmmoUI();

        isReloading = false;
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = currentAmmo + " / " + magazineSize;
    }
}