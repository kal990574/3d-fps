using UnityEngine;

public enum EDamageType
{
    Bullet,
    Explosion,
    Melee
}

public struct DamageInfo
{
    public float Damage;
    public Vector3 SourcePosition;
    public EDamageType Type;
    public bool ApplyKnockback;
    public float KnockbackStrength;

    public DamageInfo(float damage, Vector3 sourcePosition, EDamageType type = EDamageType.Bullet, float knockbackStrength = 1f)
    {
        Damage = damage;
        SourcePosition = sourcePosition;
        Type = type;
        ApplyKnockback = true;
        KnockbackStrength = knockbackStrength;
    }
}