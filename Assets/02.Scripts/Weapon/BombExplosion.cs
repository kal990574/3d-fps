using UnityEngine;

public static class BombExplosion
{
    public static void Execute(
        Vector3 position,
        float radius,
        float damage,
        LayerMask damageLayer,
        GameObject effectPrefab)
    {
        CreateExplosionEffect(position, effectPrefab);
        ApplyDamageInRadius(position, radius, damage, damageLayer);
    }

    private static void CreateExplosionEffect(Vector3 position, GameObject effectPrefab)
    {
        if (effectPrefab == null)
        {
            return;
        }

        Object.Instantiate(effectPrefab, position, Quaternion.identity);
    }

    private static void ApplyDamageInRadius(
        Vector3 position,
        float radius,
        float damage,
        LayerMask damageLayer)
    {
        Collider[] hits = Physics.OverlapSphere(position, radius, damageLayer);

        foreach (Collider hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable == null)
            {
                continue;
            }

            float distance = Vector3.Distance(position, hit.transform.position);
            float damageRatio = 1f - (distance / radius);
            float finalDamage = damage * Mathf.Max(0f, damageRatio);

            damageable.TakeDamage(finalDamage);
        }
    }
}
