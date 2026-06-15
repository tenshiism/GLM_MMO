using UnityEngine;
using System.Collections.Generic;

public class WeaponBase : MonoBehaviour
{
    [Header("Configuration")]
    public WeaponDefinition definition;

    [Header("State")]
    public int currentAmmo;
    public List<WeaponModDefinition> equippedMods = new List<WeaponModDefinition>();

    [Header("Charge (Bow)")]
    public float chargeTime;
    public float maxChargeMultiplier = 2f;

    protected float fireCooldown;
    protected bool isReloading;
    private float reloadTimer;

    public bool CanFire => currentAmmo > 0 && fireCooldown <= 0f && !isReloading;
    public bool IsReloading => isReloading;
    public bool IsCharging { get; protected set; }
    public int MaxAmmo => definition != null ? definition.magazineSize : 0;
    public float ReloadProgress => isReloading ? 1f - (reloadTimer / definition.reloadTime) : 1f;
    public float ChargeProgress => IsCharging ? Mathf.Clamp01(chargeTime / 1.5f) : 0f;

    private void Start()
    {
        if (definition != null)
            currentAmmo = definition.magazineSize;
    }

    private void Update()
    {
        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;

        if (definition != null && definition.fireType == WeaponFireType.Charge && IsCharging)
        {
            chargeTime += Time.deltaTime;
        }

        if (isReloading)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0f)
            {
                currentAmmo = definition.magazineSize;
                isReloading = false;
            }
        }
    }

    public virtual void Fire(Vector3 origin, Vector3 direction, LayerMask hitMask)
    {
        if (definition == null) return;

        if (definition.fireType == WeaponFireType.Charge)
        {
            if (!CanFire) return;
            IsCharging = true;
            chargeTime = 0f;
            return;
        }

        if (!CanFire) return;

        fireCooldown = 1f / definition.fireRate;
        currentAmmo--;

        float totalDamage = definition.damage;
        float totalSpread = definition.spread;
        float totalRange = definition.range;

        foreach (var mod in equippedMods)
        {
            if (mod == null) continue;
            totalDamage += mod.damageBonus;
            totalSpread *= mod.spreadMultiplier;
        }

        Vector3 spreadDir = direction;
        if (totalSpread > 0f)
        {
            spreadDir += Random.insideUnitSphere * totalSpread * 0.01f;
            spreadDir.Normalize();
        }

        if (Physics.Raycast(origin, spreadDir, out RaycastHit hit, totalRange, hitMask))
        {
            var hittable = hit.collider.GetComponent<IHittable>();
            if (hittable != null)
            {
                hittable.TakeDamage(totalDamage);
                hittable.OnHit(hit.point, hit.normal);
            }
        }
    }

    public void ReleaseCharge(Vector3 origin, Vector3 direction, LayerMask hitMask)
    {
        if (!IsCharging) return;
        FireCharged(origin, direction, hitMask);
    }

    private void FireCharged(Vector3 origin, Vector3 direction, LayerMask hitMask)
    {
        IsCharging = false;
        if (!CanFire) return;

        fireCooldown = 1f / definition.fireRate;
        currentAmmo--;

        float chargeRatio = Mathf.Clamp01(chargeTime / 1.5f);
        float totalDamage = definition.damage * (1f + (maxChargeMultiplier - 1f) * chargeRatio);
        float totalRange = definition.range;

        foreach (var mod in equippedMods)
        {
            if (mod == null) continue;
            totalDamage += mod.damageBonus;
        }

        if (Physics.Raycast(origin, direction, out RaycastHit hit, totalRange, hitMask))
        {
            var hittable = hit.collider.GetComponent<IHittable>();
            if (hittable != null)
            {
                hittable.TakeDamage(totalDamage);
                hittable.OnHit(hit.point, hit.normal);
            }
        }
    }

    public virtual void Reload()
    {
        if (isReloading || currentAmmo >= MaxAmmo || definition == null) return;
        isReloading = true;
        reloadTimer = definition.reloadTime;
    }

    public bool CanEquipMod(WeaponModDefinition mod)
    {
        if (mod == null) return false;
        return equippedMods.Count < definition.maxModSlots;
    }

    public bool EquipMod(WeaponModDefinition mod)
    {
        if (!CanEquipMod(mod)) return false;
        equippedMods.Add(mod);
        return true;
    }

    public bool RemoveMod(WeaponModDefinition mod)
    {
        return equippedMods.Remove(mod);
    }
}
