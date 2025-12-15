using UnityEngine;

public class Monster : MonoBehaviour
{
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
            _health.OnKnockbackComplete += HandleKnockbackComplete;
        }
    }

    private void UnsubscribeFromHealthEvents()
    {
        if (_health != null)
        {
            _health.OnDamaged -= HandleDamaged;
            _health.OnDeath -= HandleDeath;
            _health.OnKnockbackComplete -= HandleKnockbackComplete;
        }
    }

    private void HandleDamaged(Vector3 damageSourcePosition, float knockbackStrength)
    {
        if (_stateMachine != null && _stateMachine.CurrentStateType != EMonsterState.Death)
        {
            _stateMachine.ChangeState(EMonsterState.Hit);
        }
    }

    private void HandleDeath()
    {
        if (_stateMachine != null)
        {
            _stateMachine.ChangeState(EMonsterState.Death);
        }
    }

    private void HandleKnockbackComplete()
    {
        if (_stateMachine != null && _stateMachine.CurrentStateType == EMonsterState.Hit)
        {
            _stateMachine.ChangeState(EMonsterState.Idle);
        }
    }
}