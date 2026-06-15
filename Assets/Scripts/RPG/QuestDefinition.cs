using UnityEngine;
using System.Collections.Generic;

public enum QuestType
{
    Bounty,
    Survival,
    Collection,
    Boss
}

[CreateAssetMenu(menuName = "Turok26/Quest Definition")]
public class QuestDefinition : ScriptableObject
{
    public string questName;
    [TextArea] public string description;
    public QuestType questType;
    public int minLevel;
    public int maxPlayers = 4;
    public string targetScene = "SampleScene";
    public int killTarget;
    public string collectItemId;
    public int collectCount;
    public int xpReward = 100;
    public int creditReward = 500;

    [Header("Loot")]
    public List<ItemStack> guaranteedRewards = new List<ItemStack>();
    public LootTable bonusLootTable;
}
