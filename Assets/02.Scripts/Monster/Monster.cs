using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("Knockback Settings")]
    [SerializeField] private float _knockbackDistance = 2f;

    [Header("Coin Drop")]
    [SerializeField] private int _minCoinDrop = 1;
    [SerializeField] private int _maxCoinDrop = 3;

    private MonsterStateMachine _stateMachine;
    private MonsterHealth _health;

    private void Awake()
    {
        _stateMachine = GetComponent<MonsterStateMachine>();
        _health = GetComponent<MonsterHealth>();
    }

    private void Start()
    {
        SubscribeToHealthEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromHealthEvents();
    }

    private void SubscribeToHealthEvents()
    {
        if (_health != null)
        {
            _health.OnDamaged += HandleDamaged;
            _health.OnDeath += HandleDeath;
        }
    }

    private void UnsubscribeFromHealthEvents()
    {
        if (_health != null)
        {
            _health.OnDamaged -= HandleDamaged;
            _health.OnDeath -= HandleDeath;
        }
    }

    private void HandleDamaged(Vector3 damageSourcePosition, float knockbackStrength)
    {
        if (_stateMachine == null || _stateMachine.CurrentStateType == EMonsterState.Death)
        {
            return;
        }

        Vector3 knockbackDirection = CalculateKnockbackDirection(damageSourcePosition);
        float knockbackDistance = _knockbackDistance * knockbackStrength;

        _stateMachine.SetPendingKnockback(knockbackDirection, knockbackDistance);
        _stateMachine.ChangeState(EMonsterState.Hit);
    }

    private void HandleDeath()
    {
        if (_stateMachine != null)
        {
            _stateMachine.ChangeState(EMonsterState.Death);
        }

        int coinCount = Random.Range(_minCoinDrop, _maxCoinDrop + 1);
        CoinSpawner.SpawnCoins(transform.position, coinCount);
    }

    private Vector3 CalculateKnockbackDirection(Vector3 damageSourcePosition)
    {
        Vector3 direction = (transform.position - damageSourcePosition).normalized;
        direction.y = 0;
        return direction;
    }
}