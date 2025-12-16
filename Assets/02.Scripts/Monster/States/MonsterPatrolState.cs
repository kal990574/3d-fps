using UnityEngine;

public class MonsterPatrolState : IMonsterState
{
    private const float PatrolSpeedMultiplier = 0.5f;
    private const float PatrolArrivalThreshold = 1f;

    private readonly MonsterStateMachine _stateMachine;
    private Vector3 _patrolTarget;
    private bool _hasValidTarget;

    public MonsterPatrolState(MonsterStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _hasValidTarget = SetRandomPatrolTarget();

        if (!_hasValidTarget)
        {
            _stateMachine.ChangeState(EMonsterState.Idle);
        }
    }

    public void Execute()
    {
        if (!_hasValidTarget)
        {
            return;
        }

        if (_stateMachine.Movement.IsKnockbackActive)
        {
            return;
        }

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
        _stateMachine.Movement.StopMovement();
    }

    private bool SetRandomPatrolTarget()
    {
        return _stateMachine.Movement.TryGetRandomNavMeshPoint(
            _stateMachine.OriginPosition,
            _stateMachine.PatrolRadius,
            out _patrolTarget
        );
    }

    private bool HasReachedPatrolTarget()
    {
        return _stateMachine.Movement.GetHorizontalDistanceTo(_patrolTarget) < PatrolArrivalThreshold;
    }
}