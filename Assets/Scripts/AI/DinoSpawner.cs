using UnityEngine;
using System.Collections.Generic;

public class DinoSpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public int spawnCount = 3;
    public float spawnRadius = 10f;

    private void Start()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0) return;

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            pos.y = GetTerrainHeight(pos);
            Instantiate(prefab, pos, Quaternion.identity);
        }
    }

    private float GetTerrainHeight(Vector3 pos)
    {
        if (Physics.Raycast(new Vector3(pos.x, 100f, pos.z), Vector3.down, out RaycastHit hit, 200f))
            return hit.point.y;
        return pos.y;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
