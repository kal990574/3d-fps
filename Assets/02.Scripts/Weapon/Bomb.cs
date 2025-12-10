using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float _explosionDelay = 10f;
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] private float _explosionDamage = 50f;
    [SerializeField] private LayerMask _damageLayer;

    [Header("Effects")]
    [SerializeField] private GameObject _explosionEffectPrefab;

    private Rigidbody _rigidbody;
    private float _timer;
    private bool _hasExploded = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _timer = _explosionDelay;
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
        _hasExploded = true;

        BombExplosion.Execute(
            transform.position,
            _explosionRadius,
            _explosionDamage,
            _damageLayer,
            _explosionEffectPrefab
        );

        Destroy(gameObject);
    }
}
