using UnityEngine;

public class MonsterJumpState : IMonsterState
{
    private readonly MonsterStateMachine _stateMachine;
    private readonly EMonsterState _previousState;

    public MonsterJumpState(MonsterStateMachine stateMachine, EMonsterState previousState)
    {
        _stateMachine = stateMachine;
        _previousState = previousState;
    }

    public void Enter()
    {
        if (_stateMachine.Movement.TryGetCurrentLinkEndPosition(out Vector3 endPosition))
        {
            _stateMachine.Movement.StartJump(endPosition);
        }
        else
        {
            _stateMachine.ChangeState(_previousState);
        }
    }

    public void Execute()
    {
        if (!_stateMachine.Movement.IsJumping)
        {
            _stateMachine.Movement.CompleteOffMeshLink();
            _stateMachine.ChangeState(_previousState);
        }
    }

    public void Exit()
    {
    }
}