using UnityEngine;

public class MonsterIdleState : IMonsterState
{
    private readonly MonsterStateMachine _stateMachine;
    private float _idleTimer;

    public MonsterIdleState(MonsterStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _idleTimer = 0f;
    }

    public void Execute()
    {
        if (_stateMachine.IsPlayerInRange(_stateMachine.DetectDistance))
        {
            _stateMachine.ChangeState(EMonsterState.Trace);
            return;
        }

        _idleTimer += Time.deltaTime;
        if (_idleTimer >= _stateMachine.PatrolIdleTime)
        {
            _stateMachine.ChangeState(EMonsterState.Patrol);
        }
    }

    public void Exit()
    {
    }
}