using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class WeaponManager : NetworkBehaviour
{
    [Header("Starting Loadout")]
    public List<WeaponDefinition> startingWeapons;
    public Transform weaponParent;
    public LayerMask hitMask = -1;

    private readonly WeaponBase[] weapons = new WeaponBase[3];
    private int activeSlot;

    public WeaponBase ActiveWeapon
    {
        get
        {
            if (activeSlot >= 0 && activeSlot < weapons.Length)
                return weapons[activeSlot];
            return null;
        }
    }

    public int ActiveSlot => activeSlot;

    private void Start()
    {
        for (int i = 0; i < startingWeapons.Count && i < 3; i++)
            EquipWeapon(startingWeapons[i], (WeaponSlot)i);

        SwitchToSlot(0);
    }

    public void EquipWeapon(WeaponDefinition def, WeaponSlot slot)
    {
        if (def == null || def.weaponPrefab == null || weaponParent == null) return;

        int idx = (int)slot;

        if (weapons[idx] != null)
            Destroy(weapons[idx].gameObject);

        var go = Instantiate(def.weaponPrefab, weaponParent);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;

        var weapon = go.GetComponent<WeaponBase>();
        if (weapon == null)
            weapon = go.AddComponent<WeaponBase>();

        weapon.definition = def;
        weapons[idx] = weapon;
        go.SetActive(false);
    }

    public void SwitchToSlot(int slot)
    {
        if (slot < 0 || slot >= weapons.Length) return;
        if (weapons[slot] == null) return;

        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
                weapons[i].gameObject.SetActive(i == slot);
        }

        activeSlot = slot;
    }

    public void Fire(Vector3 origin, Vector3 direction)
    {
        ActiveWeapon?.Fire(origin, direction, hitMask);
    }

    public void Reload()
    {
        ActiveWeapon?.Reload();
    }

    public void SwitchNext()
    {
        int next = activeSlot;
        for (int i = 0; i < weapons.Length; i++)
        {
            next = (next + 1) % weapons.Length;
            if (weapons[next] != null)
            {
                SwitchToSlot(next);
                return;
            }
        }
    }
}
