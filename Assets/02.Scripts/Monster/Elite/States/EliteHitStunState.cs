using UnityEngine;

public class EliteHitStunState : IMonsterState
{
    private const float HitStunDuration = 0.3f;

    private readonly EliteMonsterStateMachine _stateMachine;
    private float _hitStunTimer;

    public EliteHitStunState(EliteMonsterStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _stateMachine.AnimationController?.TriggerHitAnimation();
        _hitStunTimer = 0f;
        _stateMachine.Movement.StopMovement();

        Vector3 knockbackDirection = _stateMachine.PendingKnockbackDirection;
        float knockbackDistance = _stateMachine.PendingKnockbackDistance;

        if (knockbackDistance > 0f)
        {
            _stateMachine.Movement.ApplyKnockback(knockbackDirection, knockbackDistance);
        }

        _stateMachine.ClearPendingKnockback();
    }

    public void Execute()
    {
        _hitStunTimer += Time.deltaTime;

        if (_hitStunTimer >= HitStunDuration && !_stateMachine.Movement.IsKnockbackActive)
        {
            _stateMachine.ChangeState(EEliteMonsterState.Trace);
        }
    }

    public void Exit()
    {
    }
}