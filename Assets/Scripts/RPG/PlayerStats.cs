using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterLevel))]
[RequireComponent(typeof(Inventory))]
public class PlayerStats : NetworkBehaviour
{
    [Header("Base Stats")]
    public float baseMaxHealth = 100f;
    public float baseMoveSpeed = 6f;
    public float baseDamageBonus = 0f;

    [Header("Per-Level Scaling")]
    public float healthPerLevel = 10f;
    public float damagePerLevel = 2f;
    public float speedPerLevel = 0.1f;

    private CharacterLevel charLevel;

    private void Awake()
    {
        charLevel = GetComponent<CharacterLevel>();
    }

    public float MaxHealth => baseMaxHealth + (charLevel.Level.Value - 1) * healthPerLevel;
    public float DamageBonus => baseDamageBonus + (charLevel.Level.Value - 1) * damagePerLevel;
    public float MoveSpeed => baseMoveSpeed + (charLevel.Level.Value - 1) * speedPerLevel;
    public int CurrentLevel => charLevel.Level.Value;
}
