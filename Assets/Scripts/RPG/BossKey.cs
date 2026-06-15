using UnityEngine;

public class BossKey : ItemDefinition
{
    [Header("Boss Key")]
    public string bossZoneId;
    public string bossName;

    public override string GetTooltip()
    {
        return $"<b>{itemName}</b>\n<color=grey>{rarity}</color>\n{description}\n<color=yellow>Unlocks: {bossName}</color>";
    }
}
