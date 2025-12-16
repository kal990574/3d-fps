using UnityEngine;

public class MonsterTraceState : IMonsterState
{
    private readonly MonsterStateMachine _stateMachine;

    public MonsterTraceState(MonsterStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
    }

    public void Execute()
    {
        if (_stateMachine.Movement.IsKnockbackActive)
        {
            return;
        }

        float distance = _stateMachine.GetDistanceToPlayer();

        if (distance > _stateMachine.MaxTraceDistance)
        {
            _stateMachine.ChangeState(EMonsterState.Comeback);
            return;
        }

        if (distance <= _stateMachine.Combat.AttackDistance)
        {
            _stateMachine.ChangeState(EMonsterState.Attack);
            return;
        }

        Vector3 playerPosition = _stateMachine.Combat.GetPlayerPosition();
        _stateMachine.Movement.MoveTowards(playerPosition);
        _stateMachine.Movement.LookAtTarget(playerPosition);
    }

    public void Exit()
    {
        _stateMachine.Movement.StopMovement();
    }
}