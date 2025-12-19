using UnityEngine;

public class MonsterAttackState : IMonsterState
{
    private readonly MonsterStateMachine _stateMachine;

    public MonsterAttackState(MonsterStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _stateMachine.Combat.ResetAttackTimer();
    }

    public void Execute()
    {
        float distanceToPlayer = _stateMachine.GetDistanceToPlayer();

        if (distanceToPlayer > _stateMachine.Combat.AttackDistance)
        {
            _stateMachine.ChangeState(EMonsterState.Trace);
            return;
        }

        Vector3 playerPosition = _stateMachine.Combat.GetPlayerPosition();
        _stateMachine.Movement.LookAtTarget(playerPosition);

        if (_stateMachine.Combat.CanAttack())
        {
            _stateMachine.Combat.TriggerAttack();
        }
    }

    public void Exit()
    {
    }
}