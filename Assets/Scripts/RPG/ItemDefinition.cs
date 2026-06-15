using UnityEngine;

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public abstract class ItemDefinition : ScriptableObject
{
    public string itemName;
    [TextArea(2, 4)] public string description;
    public Sprite icon;
    public ItemRarity rarity = ItemRarity.Common;
    public int maxStackSize = 1;
    public int buyPrice;
    public int sellPrice;

    public virtual string GetTooltip()
    {
        return $"<b>{itemName}</b>\n<color=grey>{rarity}</color>\n{description}";
    }
}
