using UnityEngine;

public class EliteAttackState : IMonsterState
{
    private const float AttackDuration = 1.5f;
    private const float DamageTime = 0.8f;

    private readonly EliteMonsterStateMachine _stateMachine;
    private float _attackTimer;
    private bool _damageDealt;

    public EliteAttackState(EliteMonsterStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _attackTimer = 0f;
        _damageDealt = false;
        _stateMachine.RecordAttackUsed();
        _stateMachine.Combat.TriggerAttack();
    }

    public void Execute()
    {
        _attackTimer += Time.deltaTime;

        Vector3 playerPosition = _stateMachine.Combat.GetPlayerPosition();
        _stateMachine.Movement.LookAtTarget(playerPosition);

        if (!_damageDealt && _attackTimer >= DamageTime)
        {
            _stateMachine.Combat.ExecuteAttack();
            _damageDealt = true;
        }

        if (_attackTimer >= AttackDuration)
        {
            float distanceToPlayer = _stateMachine.GetDistanceToPlayer();

            if (distanceToPlayer <= _stateMachine.Combat.AttackDistance && _stateMachine.CanAttack())
            {
                _stateMachine.ChangeState(EEliteMonsterState.Attack);
            }
            else
            {
                _stateMachine.ChangeState(EEliteMonsterState.Trace);
            }
        }
    }

    public void Exit()
    {
    }
}
