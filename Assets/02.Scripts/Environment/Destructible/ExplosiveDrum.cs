using UnityEngine;

public class ExplosiveDrum : DestructibleBase
{
    [Header("Explosion Settings")]
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] private float _explosionDamage = 80f;
    [SerializeField] private LayerMask _hitLayers;

    [Header("Launch Settings")]
    [SerializeField] private float _launchForce = 15f;
    [SerializeField] private float _torqueForce = 5f;

    private Rigidbody _rigidbody;

    protected override void Awake()
    {
        base.Awake();
        _rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnDestruct()
    {
        DealExplosionDamage();
        PlayExplosionEffect();
        LaunchDrum();
    }

    private void DealExplosionDamage()
    {
        ExplosionData data = new ExplosionData(
            transform.position,
            _explosionRadius,
            _explosionDamage,
            _hitLayers
        );
        ExplosionDamage.Explode(data);
    }

    private void PlayExplosionEffect()
    {
        EffectPoolManager.Instance?.Play("DrumExplosion", transform.position);
    }

    private void LaunchDrum()
    {
        if (_rigidbody == null)
        {
            return;
        }

        _rigidbody.isKinematic = false;

        Vector3 launchDir = Vector3.up + Random.insideUnitSphere * 0.3f;
        _rigidbody.AddForce(launchDir.normalized * _launchForce, ForceMode.VelocityChange);

        Vector3 randomTorque = Random.insideUnitSphere * _torqueForce;
        _rigidbody.AddTorque(randomTorque, ForceMode.VelocityChange);
    }
}