using UnityEngine.AI;

public class MonsterDeadState : IMonsterState
{
    private readonly MonsterStateMachine _stateMachine;

    public MonsterDeadState(MonsterStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _stateMachine.Movement.StopMovement();

        NavMeshAgent agent = _stateMachine.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false;
        }
    }

    public void Execute()
    {
    }

    public void Exit()
    {
    }
}