using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [Header("Boss Config")]
    public GameObject bossPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 60f;
    public int maxBosses = 1;
    public bool spawnOnStart = true;
    public bool scaleWithPlayerLevel = true;

    [Header("Level Scaling")]
    public float healthPerLevel = 15f;
    public float damagePerLevel = 3f;

    private int activeBosses;
    private float nextSpawnTime;

    private void Start()
    {
        if (spawnOnStart)
            SpawnBoss();
        nextSpawnTime = Time.time + spawnInterval;
    }

    private void Update()
    {
        if (activeBosses >= maxBosses) return;
        if (Time.time >= nextSpawnTime)
        {
            SpawnBoss();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    public void SpawnBoss()
    {
        if (bossPrefab == null) return;
        if (activeBosses >= maxBosses) return;

        Vector3 pos = GetSpawnPosition();
        var go = Instantiate(bossPrefab, pos, Quaternion.identity);
        activeBosses++;

        var player = GameObject.FindGameObjectWithTag("Player");
        var playerLevel = 1;
        if (player != null)
        {
            var cl = player.GetComponent<CharacterLevel>();
            if (cl != null) playerLevel = cl.Level.Value;
        }

        var brain = go.GetComponent<EnemyBrain>();
        if (brain != null)
        {
            if (scaleWithPlayerLevel)
            {
                brain.maxHealth += healthPerLevel * (playerLevel - 1);
                brain.damage += damagePerLevel * (playerLevel - 1);
            }
            brain.OnDied += () => activeBosses--;
        }

        var tint = go.GetComponent<EnemyTint>();
        if (tint != null)
            tint.ApplyTint(playerLevel);
    }

    private Vector3 GetSpawnPosition()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            var pt = spawnPoints[Random.Range(0, spawnPoints.Length)];
            if (pt != null) return pt.position;
        }
        return transform.position;
    }
}
