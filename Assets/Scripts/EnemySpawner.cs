using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy")]
    public GameObject enemyPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Settings")]
    public int maxEnemies = 10;
    public float respawnTime = 3f;

    private int currentEnemies = 0;

    private List<Transform> usedSpawns = new List<Transform>();

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (currentEnemies < maxEnemies)
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(respawnTime);
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null || spawnPoints.Length == 0)
            return;

        Transform spawn = GetFreeSpawnPoint();

        if (spawn == null)
            return;

        GameObject enemy = Instantiate(enemyPrefab, spawn.position, spawn.rotation);

        // 🧱 blokada miejsca
        usedSpawns.Add(spawn);

        currentEnemies++;

        EnemyHealth hp = enemy.GetComponent<EnemyHealth>();
        if (hp != null)
        {
            hp.onDeath += () =>
            {
                currentEnemies--;
                usedSpawns.Remove(spawn);
            };
        }

        // 🧍‍♂️ FIX: NIE WYWRACANIE
        Rigidbody rb = enemy.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.freezeRotation = true;
        }
    }

    Transform GetFreeSpawnPoint()
    {
        List<Transform> free = new List<Transform>();

        foreach (Transform t in spawnPoints)
        {
            if (!usedSpawns.Contains(t))
                free.Add(t);
        }

        if (free.Count == 0)
            return null;

        return free[Random.Range(0, free.Count)];
    }
}