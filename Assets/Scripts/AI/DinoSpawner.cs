using UnityEngine;
using System.Collections.Generic;

public class DinoSpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public int spawnCount = 5;
    public float spawnRadius = 15f;
    public float spawnInterval = 30f;
    public bool spawnOnStart = true;
    public bool respawnEnabled = true;

    private readonly List<GameObject> spawned = new List<GameObject>();
    private float nextSpawnTime;

    private void Start()
    {
        if (spawnOnStart)
            SpawnWave();
        nextSpawnTime = Time.time + spawnInterval;
    }

    private void Update()
    {
        if (!respawnEnabled) return;
        spawned.RemoveAll(s => s == null);

        if (Time.time >= nextSpawnTime && spawned.Count < spawnCount)
        {
            int toSpawn = spawnCount - spawned.Count;
            for (int i = 0; i < toSpawn; i++)
                SpawnOne();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    public void SpawnWave()
    {
        for (int i = 0; i < spawnCount; i++)
            SpawnOne();
    }

    private void SpawnOne()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0) return;

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        Vector3 pos = GetValidSpawnPosition();
        var go = Instantiate(prefab, pos, Quaternion.identity);
        spawned.Add(go);
    }

    private Vector3 GetValidSpawnPosition()
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector3 candidate = transform.position + Random.insideUnitSphere * spawnRadius;
            candidate.y = GetTerrainHeight(candidate);

            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(candidate, out hit, spawnRadius, UnityEngine.AI.NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        Vector3 fallback = transform.position;
        fallback.y = GetTerrainHeight(fallback);
        return fallback;
    }

    private float GetTerrainHeight(Vector3 pos)
    {
        if (Physics.Raycast(new Vector3(pos.x, 100f, pos.z), Vector3.down, out RaycastHit hit, 200f, LayerMask.GetMask("Environment")))
            return hit.point.y;
        return pos.y;
    }

    public void ClearSpawned()
    {
        foreach (var go in spawned)
        {
            if (go != null) Destroy(go);
        }
        spawned.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
