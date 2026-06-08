using UnityEngine;

public interface IHittable
{
    void TakeDamage(float amount);
    void OnHit(Vector3 hitPoint, Vector3 hitDirection);
}
