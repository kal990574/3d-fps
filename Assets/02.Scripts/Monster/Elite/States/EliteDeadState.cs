using UnityEngine.AI;

public class EliteDeadState : IMonsterState
{
    private readonly EliteMonsterStateMachine _stateMachine;

    public EliteDeadState(EliteMonsterStateMachine stateMachine)
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
