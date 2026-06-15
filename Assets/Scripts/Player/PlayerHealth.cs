using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(PlayerStats))]
public class PlayerHealth : NetworkBehaviour, IHittable
{
    [Header("Regen")]
    public float regenDelay = 5f;
    public float regenRate = 2f;

    public NetworkVariable<float> CurrentHealth = new NetworkVariable<float>(100f);
    public System.Action OnDied;
    public System.Action<float> OnHealthChanged;

    private PlayerStats stats;
    private float lastDamageTime;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    private void Start()
    {
        if (!IsSpawned)
            CurrentHealth.Value = stats.MaxHealth;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            CurrentHealth.Value = stats.MaxHealth;
    }

    private void Update()
    {
        if (CurrentHealth.Value <= 0f) return;

        if (Time.time - lastDamageTime > regenDelay && CurrentHealth.Value < stats.MaxHealth)
        {
            CurrentHealth.Value = Mathf.Min(CurrentHealth.Value + regenRate * Time.deltaTime, stats.MaxHealth);
            OnHealthChanged?.Invoke(CurrentHealth.Value);
        }
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth.Value -= amount;
        lastDamageTime = Time.time;
        OnHealthChanged?.Invoke(CurrentHealth.Value);

        if (CurrentHealth.Value <= 0f)
            Die();
    }

    public void OnHit(Vector3 hitPoint, Vector3 hitDirection) { }

    public void Heal(float amount)
    {
        CurrentHealth.Value = Mathf.Min(CurrentHealth.Value + amount, stats.MaxHealth);
        OnHealthChanged?.Invoke(CurrentHealth.Value);
    }

    private void Die()
    {
        CurrentHealth.Value = 0f;
        OnDied?.Invoke();
        gameObject.SetActive(false);
    }
}
