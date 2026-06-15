using UnityEngine;
using System.Collections.Generic;

public class LootDropper : MonoBehaviour
{
    [Header("Loot")]
    public LootTable lootTable;
    public GameObject pickupPrefab;
    public float spreadRadius = 1.5f;

    [Header("XP")]
    public int xpReward = 25;

    public void SpawnLoot(Vector3 position)
    {
        if (lootTable != null)
        {
            var drops = lootTable.Roll();
            foreach (var stack in drops)
                SpawnPickup(position, stack);
            xpReward = lootTable.xpReward;
        }
    }

    private void SpawnPickup(Vector3 position, ItemStack stack)
    {
        if (pickupPrefab == null) return;

        Vector3 offset = Random.insideUnitSphere * spreadRadius;
        offset.y = 0f;
        Vector3 spawnPos = position + offset;

        if (Physics.Raycast(spawnPos + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 10f))
            spawnPos = hit.point + Vector3.up * 0.5f;

        var go = Instantiate(pickupPrefab, spawnPos, Quaternion.identity);
        var pickup = go.GetComponent<ItemPickup>();
        if (pickup != null)
            pickup.SetItem(stack);
    }
}
