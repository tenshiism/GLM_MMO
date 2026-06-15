using UnityEngine;

public enum ConsumableType
{
    Health,
    Ammo,
    Stim
}

[CreateAssetMenu(menuName = "Turok26/Items/Consumable")]
public class ConsumableItemDefinition : ItemDefinition
{
    public ConsumableType consumableType;
    public float value;
    public float useTime = 1f;
}
