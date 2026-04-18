using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    [Header("HP")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Death")]
    public float destroyTime = 2f;

    public Action onDeath;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        onDeath?.Invoke();
        Destroy(gameObject, destroyTime);
    }
}