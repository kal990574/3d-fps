using UnityEngine;

public static class BombExplosion
{
    public static void Execute(Vector3 position, IEffectPool effectPool)
    {
        PlayExplosionEffect(position, effectPool);
    }

    private static void PlayExplosionEffect(Vector3 position, IEffectPool effectPool)
    {
        if (effectPool == null)
        {
            return;
        }

        effectPool.Play(position);
    }
}
