using UnityEngine;

public class GrenadeWeapon : WeaponBase
{
    [Header("Grenade")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 20f;
    public float explosionRadius = 5f;
    public float explosionDamage = 30f;
    public float explosionLifetime = 3f;
    public LayerMask damageMask = -1;

    public override void Fire(Vector3 origin, Vector3 direction, LayerMask hitMask)
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

        GameObject proj = Instantiate(projectilePrefab, origin, Quaternion.LookRotation(direction));
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = direction * projectileSpeed;

        GrenadeProjectile gp = proj.GetComponent<GrenadeProjectile>();
        if (gp != null)
        {
            gp.explosionRadius = explosionRadius;
            gp.explosionDamage = explosionDamage;
            gp.lifetime = explosionLifetime;
            gp.damageMask = damageMask;
            gp.owner = gameObject;
        }
    }
}
