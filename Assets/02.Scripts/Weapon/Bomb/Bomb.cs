using UnityEngine;

public class Bomb : MonoBehaviour, IPoolable
{
    [Header("Explosion Settings")]
    [SerializeField] private float _explosionDelay = 10f;
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] private float _explosionDamage = 100f;
    [SerializeField] private LayerMask _hitLayers;

    private Rigidbody _rigidbody;
    private float _timer;
    private bool _hasExploded;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _timer = _explosionDelay;
        _hasExploded = false;
    }

    public void OnGetFromPool() { }

    public void OnReturnToPool()
    {
        ResetState();
    }

    private void ResetState()
    {
        _hasExploded = false;
        _timer = _explosionDelay;
        ResetRigidbody();
    }

    private void ResetRigidbody()
    {
        if (_rigidbody == null)
        {
            return;
        }

        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    public void Launch(Vector3 direction, float force)
    {
        if (_rigidbody == null)
        {
            Debug.LogError("Bomb is missing Rigidbody component!");
            return;
        }

        _rigidbody.AddForce(direction * force, ForceMode.VelocityChange);
    }

    private void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if (_hasExploded)
        {
            return;
        }

        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            Explode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasExploded)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        Explode();
    }

    private void Explode()
    {
        if (_hasExploded)
        {
            return;
        }
        _hasExploded = true;

        ExplosionData explosionData = new ExplosionData(
            transform.position,
            _explosionRadius,
            _explosionDamage,
            _hitLayers
        );
        ExplosionDamage.Explode(explosionData);
        EffectPoolManager.Instance?.Play("BombExplosion", transform.position);

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        if (GameplayPoolManager.Instance != null)
        {
            GameplayPoolManager.Instance.ReleaseBomb(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
