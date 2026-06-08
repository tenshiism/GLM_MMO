using UnityEngine;

public class PlayerHealth : MonoBehaviour, IHittable
{
    public float maxHealth = 100f;
    public float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f) Die();
    }

    public void OnHit(Vector3 hitPoint, Vector3 hitDirection) { }

    private void Die()
    {
        gameObject.SetActive(false);
    }
}
