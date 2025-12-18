using UnityEngine;

public class MonsterHitState : IMonsterState
{
    private readonly MonsterStateMachine _stateMachine;

    public MonsterHitState(MonsterStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _stateMachine.Movement.StopMovement();

        Vector3 knockbackDirection = _stateMachine.PendingKnockbackDirection;
        float knockbackDistance = _stateMachine.PendingKnockbackDistance;

        if (knockbackDistance > 0f)
        {
            _stateMachine.Movement.ApplyKnockback(knockbackDirection, knockbackDistance);
        }

        _stateMachine.ClearPendingKnockback();
    }

    public void Execute()
    {
        if (!_stateMachine.Movement.IsKnockbackActive)
        {
            _stateMachine.ChangeState(EMonsterState.Idle);
        }
    }

    public void Exit()
    {
    }
}