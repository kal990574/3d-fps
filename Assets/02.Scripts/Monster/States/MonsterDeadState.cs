public class MonsterDeadState : IMonsterState
{
    private readonly MonsterStateMachine _stateMachine;

    public MonsterDeadState(MonsterStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
    }

    public void Execute()
    {
    }

    public void Exit()
    {
    }
}