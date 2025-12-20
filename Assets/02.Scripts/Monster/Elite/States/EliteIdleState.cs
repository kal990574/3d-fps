using UnityEngine;

public class EliteIdleState : IMonsterState
{
    private readonly EliteMonsterStateMachine _stateMachine;

    public EliteIdleState(EliteMonsterStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
    }

    public void Execute()
    {
        _stateMachine.ChangeState(EEliteMonsterState.Trace);
    }

    public void Exit()
    {
    }
}
