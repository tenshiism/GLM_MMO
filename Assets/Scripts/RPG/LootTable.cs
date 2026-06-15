using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct LootEntry
{
    public string itemId;
    [Range(0f, 1f)] public float dropChance;
    public int minQuantity;
    public int maxQuantity;
}

[CreateAssetMenu(menuName = "Turok26/LootTable")]
public class LootTable : ScriptableObject
{
    public List<LootEntry> entries;
    public int guaranteedDrops = 1;
    public int xpReward = 25;

    public List<ItemStack> Roll()
    {
        var drops = new List<ItemStack>();
        int attempts = Mathf.Max(guaranteedDrops, entries.Count);

        foreach (var entry in entries)
        {
            if (Random.value <= entry.dropChance)
            {
                int qty = Random.Range(entry.minQuantity, entry.maxQuantity + 1);
                if (qty > 0)
                    drops.Add(new ItemStack(entry.itemId, qty));
            }
        }

        return drops;
    }
}
