using UnityEngine;

public class Coin : MonoBehaviour, IPoolable
{
    [Header("Magnet Settings")]
    [SerializeField] private float _magnetSpeed = 10f;
    [SerializeField] private float _magnetRange = 5f;
    [SerializeField] private float _collectDistance = 0.5f;

    [Header("Spawn Settings")]
    [SerializeField] private float _initialUpwardForce = 3f;
    [SerializeField] private float _gravity = 15f;
    [SerializeField] private float _groundHeight = 0.5f;

    private Transform _playerTransform;
    private Vector3 _velocity;
    private bool _isGrounded;
    private bool _isCollected;

    private void Start()
    {
        FindPlayer();
    }

    private void Update()
    {
        if (_isCollected)
        {
            return;
        }

        if (_playerTransform == null)
        {
            FindPlayer();
            return;
        }

        if (!_isGrounded)
        {
            ApplyGravity();
        }
        else
        {
            TryMagnetToPlayer();
        }
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }
    }

    private void ApplyGravity()
    {
        _velocity.y -= _gravity * Time.deltaTime;
        transform.position += _velocity * Time.deltaTime;

        if (transform.position.y <= _groundHeight)
        {
            Vector3 pos = transform.position;
            pos.y = _groundHeight;
            transform.position = pos;
            _velocity = Vector3.zero;
            _isGrounded = true;
        }
    }

    private void TryMagnetToPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

        if (distanceToPlayer <= _collectDistance)
        {
            Collect();
            return;
        }

        if (distanceToPlayer <= _magnetRange)
        {
            Vector3 direction = (_playerTransform.position - transform.position).normalized;
            float speedMultiplier = 1f + (1f - distanceToPlayer / _magnetRange);
            transform.position += direction * _magnetSpeed * speedMultiplier * Time.deltaTime;
        }
    }

    private void Collect()
    {
        if (_isCollected)
        {
            return;
        }

        _isCollected = true;
        GameEvents.TriggerCoinCollected(1);
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
            GameplayPoolManager.Instance.ReleaseCoin(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(Vector3 scatterDirection)
    {
        _velocity = scatterDirection * 2f + Vector3.up * _initialUpwardForce;
        _isGrounded = false;
        _isCollected = false;
    }

    public void OnGetFromPool()
    {
        _isGrounded = false;
        _isCollected = false;
        _velocity = Vector3.zero;
    }

    public void OnReturnToPool()
    {
        _velocity = Vector3.zero;
        _isGrounded = false;
        _isCollected = false;
    }
}
