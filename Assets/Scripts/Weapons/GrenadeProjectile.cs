using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    public float explosionRadius = 5f;
    public float explosionDamage = 30f;
    public float lifetime = 3f;
    public LayerMask damageMask = -1;
    public GameObject owner;

    private float timer;

    private void Start()
    {
        timer = lifetime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
            Explode();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    private void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, damageMask);
        foreach (var hit in hits)
        {
            if (hit.gameObject == owner) continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);
            float falloff = 1f - Mathf.Clamp01(dist / explosionRadius);

            var hittable = hit.GetComponent<IHittable>();
            if (hittable != null)
                hittable.TakeDamage(explosionDamage * falloff);
        }

        Destroy(gameObject);
    }
}
