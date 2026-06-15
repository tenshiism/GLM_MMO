using UnityEngine;

public enum WeaponFireType
{
    Semi,
    Auto,
    Burst,
    Charge
}

public enum WeaponSlot
{
    Primary,
    Secondary,
    Melee
}

[CreateAssetMenu(menuName = "Turok26/Weapon")]
public class WeaponDefinition : ScriptableObject
{
    [Header("Info")]
    public string weaponName;
    public GameObject weaponPrefab;
    public Sprite icon;

    [Header("Stats")]
    public float damage = 20f;
    public float fireRate = 2f;
    public float range = 50f;
    public int magazineSize = 30;
    public float reloadTime = 2f;
    public int maxModSlots = 3;

    [Header("Accuracy")]
    public float spread = 0f;
    public float recoilAmount = 1f;

    [Header("Type")]
    public WeaponFireType fireType = WeaponFireType.Auto;
    public WeaponSlot slot = WeaponSlot.Primary;
}
