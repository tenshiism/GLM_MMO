using UnityEngine;

public class BossRoom : Interactable
{
    [Header("Boss Room")]
    public string requiredKeyItemId = "BossKey";
    public string bossZoneId;
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;
    public float bossSpawnDelay = 2f;

    [Header("UI")]
    public string lockedPrompt = "Boss Room (Needs Key)";

    private bool bossSpawned;
    private float spawnTimer;

    private void Start()
    {
        interactPrompt = lockedPrompt;
    }

    private void Update()
    {
        if (spawnTimer > 0f)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
                SpawnBoss();
        }
    }

    public override void OnInteract(GameObject player)
    {
        if (bossSpawned) return;

        var inventory = player.GetComponent<Inventory>();
        if (inventory == null) return;

        if (!inventory.HasItem(requiredKeyItemId, 1))
        {
            Debug.Log($"Boss Room: You need a {requiredKeyItemId} to enter!");
            return;
        }

        inventory.RemoveItem(requiredKeyItemId, 1);
        Debug.Log("Boss Room: Key consumed! Boss spawning...");
        spawnTimer = bossSpawnDelay;
        interactPrompt = "Boss spawning...";
    }

    protected override void OnPlayerEnter()
    {
        if (bossSpawned) return;
        if (playerObject == null) return;

        var inventory = playerObject.GetComponent<Inventory>();
        if (inventory != null && inventory.HasItem(requiredKeyItemId, 1))
            interactPrompt = "Enter Boss Room (E)";
        else
            interactPrompt = lockedPrompt;
    }

    protected override void OnPlayerExit()
    {
        interactPrompt = lockedPrompt;
    }

    private void SpawnBoss()
    {
        if (bossPrefab == null) return;

        Vector3 spawnPos = bossSpawnPoint != null ? bossSpawnPoint.position : transform.position + Vector3.forward * 5f;
        Quaternion spawnRot = bossSpawnPoint != null ? bossSpawnPoint.rotation : Quaternion.identity;

        var boss = Instantiate(bossPrefab, spawnPos, spawnRot);
        bossSpawned = true;
        interactPrompt = "";

        var tint = boss.GetComponent<EnemyTint>();
        if (tint != null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var cl = player.GetComponent<CharacterLevel>();
                if (cl != null)
                    tint.ApplyTint(cl.Level.Value);
            }
        }

        Debug.Log($"Boss spawned: {boss.name}");
    }
}
