using UnityEngine;

public struct ExplosionData
{
    public Vector3 Position;
    public float Radius;
    public float MaxDamage;
    public LayerMask HitLayers;

    public ExplosionData(Vector3 position, float radius, float maxDamage, LayerMask hitLayers)
    {
        Position = position;
        Radius = radius;
        MaxDamage = maxDamage;
        HitLayers = hitLayers;
    }
}