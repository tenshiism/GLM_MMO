using UnityEngine;

public class DinoSpawner : MonoBehaviour
{
    public GameObject dinoPrefab;
    public int spawnCount = 3;
    public float spawnRadius = 10f;

    private void Start()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            pos.y = GetTerrainHeight(pos);
            Instantiate(dinoPrefab, pos, Quaternion.identity);
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
