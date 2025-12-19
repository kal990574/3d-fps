using UnityEngine;
using System;

public class EliteMonsterAnimationController : BaseAnimationController
{
    private EliteMonsterStateMachine _stateMachine;
    private EEliteMonsterState _previousState;

    public event Action OnAttackExecute;

    protected override void Awake()
    {
        base.Awake();
        _stateMachine = GetComponent<EliteMonsterStateMachine>();
    }

    private void Update()
    {
        if (_stateMachine == null)
        {
            return;
        }

        UpdateAnimationByState();
    }

    private void UpdateAnimationByState()
    {
        EEliteMonsterState currentState = _stateMachine.CurrentStateType;

        if (currentState != _previousState)
        {
            OnStateChanged(currentState);
            _previousState = currentState;
        }

        if (IsMovingState(currentState))
        {
            UpdateMovementAnimation();
        }
    }

    private void OnStateChanged(EEliteMonsterState newState)
    {
        switch (newState)
        {
            case EEliteMonsterState.Idle:
            case EEliteMonsterState.Trace:
            case EEliteMonsterState.Attack:
                break;
            case EEliteMonsterState.Death:
                SetBool(AnimatorParams.IsDead, true);
                break;
        }
    }

    private bool IsMovingState(EEliteMonsterState state)
    {
        return state == EEliteMonsterState.Trace;
    }

    private void UpdateMovementAnimation()
    {
        float speed = _stateMachine.Movement.CurrentSpeed;
        SetFloat(AnimatorParams.Speed, speed);
    }

    public void AnimEvent_Attack()
    {
        OnAttackExecute?.Invoke();
    }

    public void TriggerAttackAnimation()
    {
        SetTrigger(AnimatorParams.Attack);
    }
}
