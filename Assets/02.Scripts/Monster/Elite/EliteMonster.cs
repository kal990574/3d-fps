using UnityEngine;

public class EliteMonster : MonoBehaviour
{
    [Header("Knockback Settings")]
    [SerializeField] private float _knockbackDistance = 1f;

    [Header("Coin Drop")]
    [SerializeField] private int _minCoinDrop = 5;
    [SerializeField] private int _maxCoinDrop = 10;

    private EliteMonsterStateMachine _stateMachine;
    private MonsterHealth _health;

    private void Awake()
    {
        _stateMachine = GetComponent<EliteMonsterStateMachine>();
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
        if (_stateMachine == null || _stateMachine.CurrentStateType == EEliteMonsterState.Death)
        {
            return;
        }

        Vector3 knockbackDirection = CalculateKnockbackDirection(damageSourcePosition);
        float knockbackDistance = _knockbackDistance * knockbackStrength;

        if (knockbackDistance > 0f)
        {
            _stateMachine.SetPendingKnockback(knockbackDirection, knockbackDistance);
        }

        _stateMachine.ChangeState(EEliteMonsterState.HitStun);
    }

    private void HandleDeath()
    {
        if (_stateMachine != null)
        {
            _stateMachine.ChangeState(EEliteMonsterState.Death);
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
