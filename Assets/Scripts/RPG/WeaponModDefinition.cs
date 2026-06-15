using UnityEngine;

public enum ModType
{
    Damage,
    FireRate,
    Elemental,
    Scope,
    Suppressor
}

public enum ElementType
{
    None,
    Fire,
    Poison,
    Ice,
    Shock
}

[CreateAssetMenu(menuName = "Turok26/Items/WeaponMod")]
public class WeaponModDefinition : ItemDefinition
{
    public ModType modType;
    public ElementType element = ElementType.None;
    public float damageBonus;
    public float fireRateBonus;
    public float spreadMultiplier = 1f;
}
