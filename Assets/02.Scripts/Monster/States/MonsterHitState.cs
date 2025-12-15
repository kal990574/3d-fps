public class MonsterHitState : IMonsterState
{
    private readonly MonsterStateMachine _stateMachine;

    public MonsterHitState(MonsterStateMachine stateMachine)
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