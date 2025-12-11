using UnityEngine;

public static class ExplosionDamage
{
    private const float MaxKnockbackStrength = 5f;
    private const float MinKnockbackStrength = 2f;

    public static void Explode(ExplosionData data)
    {
        Collider[] hits = Physics.OverlapSphere(data.Position, data.Radius, data.HitLayers);

        foreach (Collider hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                float distance = Vector3.Distance(data.Position, hit.transform.position);
                float damagePercent = 1 - (distance / data.Radius);
                float finalDamage = data.MaxDamage * Mathf.Clamp01(damagePercent);

                float knockbackStrength = Mathf.Lerp(MaxKnockbackStrength, MinKnockbackStrength, distance / data.Radius);

                DamageInfo damageInfo = new DamageInfo(finalDamage, data.Position, EDamageType.Explosion, knockbackStrength);
                damageable.TakeDamage(damageInfo);
            }
        }
    }
}