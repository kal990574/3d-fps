using UnityEngine;

public class MonsterComebackState : IMonsterState
{
    private readonly MonsterStateMachine _stateMachine;

    public MonsterComebackState(MonsterStateMachine stateMachine)
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

        if (_stateMachine.Movement.IsOnOffMeshLink)
        {
            _stateMachine.ChangeState(EMonsterState.Jump);
            return;
        }

        _stateMachine.Movement.MoveTowards(_stateMachine.OriginPosition);
        _stateMachine.Movement.LookAtTarget(_stateMachine.OriginPosition);

        float distanceToOrigin = _stateMachine.Movement.GetDistanceTo(_stateMachine.OriginPosition);

        if (distanceToOrigin < _stateMachine.ComebackStopDistance)
        {
            _stateMachine.ChangeState(EMonsterState.Idle);
        }
    }

    public void Exit()
    {
        _stateMachine.Movement.StopMovement();
    }
}