using UnityEngine;

public class EliteTraceState : IMonsterState
{
    private readonly EliteMonsterStateMachine _stateMachine;

    public EliteTraceState(EliteMonsterStateMachine stateMachine)
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

        if (distance <= _stateMachine.Combat.AttackDistance)
        {
            _stateMachine.ChangeState(EEliteMonsterState.Attack);
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
