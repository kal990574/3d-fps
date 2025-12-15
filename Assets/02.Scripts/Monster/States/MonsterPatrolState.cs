using UnityEngine;

public class MonsterPatrolState : IMonsterState
{
    private const float PatrolSpeedMultiplier = 0.5f;
    private const float PatrolArrivalThreshold = 1f;

    private readonly MonsterStateMachine _stateMachine;
    private Vector3 _patrolTarget;

    public MonsterPatrolState(MonsterStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        SetRandomPatrolTarget();
    }

    public void Execute()
    {
        if (_stateMachine.IsPlayerInRange(_stateMachine.DetectDistance))
        {
            _stateMachine.ChangeState(EMonsterState.Trace);
            return;
        }

        _stateMachine.Movement.MoveTowards(_patrolTarget, PatrolSpeedMultiplier);
        _stateMachine.Movement.LookAtTarget(_patrolTarget);

        if (HasReachedPatrolTarget())
        {
            _stateMachine.ChangeState(EMonsterState.Idle);
        }
    }

    public void Exit()
    {
    }

    private void SetRandomPatrolTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * _stateMachine.PatrolRadius;
        _patrolTarget = _stateMachine.OriginPosition + new Vector3(randomCircle.x, 0, randomCircle.y);
    }

    private bool HasReachedPatrolTarget()
    {
        return _stateMachine.Movement.GetHorizontalDistanceTo(_patrolTarget) < PatrolArrivalThreshold;
    }
}