using UnityEngine;

public class MonsterAnimationController : BaseAnimationController
{
    private MonsterStateMachine _stateMachine;
    private EMonsterState _previousState;

    protected override void Awake()
    {
        base.Awake();
        _stateMachine = GetComponent<MonsterStateMachine>();
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
        EMonsterState currentState = _stateMachine.CurrentStateType;

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

    private void OnStateChanged(EMonsterState newState)
    {
        switch (newState)
        {
            case EMonsterState.Idle:
            case EMonsterState.Patrol:
            case EMonsterState.Trace:
            case EMonsterState.Comeback:
                // Idle/Walk는 Speed 파라미터로 Animator Transition이 처리
                break;
            case EMonsterState.Attack:
                SetTrigger(AnimatorParams.Attack);
                break;
            case EMonsterState.Hit:
                SetTrigger(AnimatorParams.Hit);
                break;
            case EMonsterState.Death:
                SetBool(AnimatorParams.IsDead, true);
                break;
            case EMonsterState.Jump:
                SetTrigger(AnimatorParams.Jump);
                break;
        }
    }

    private bool IsMovingState(EMonsterState state)
    {
        return state == EMonsterState.Patrol 
            || state == EMonsterState.Trace 
            || state == EMonsterState.Comeback;
    }

    private void UpdateMovementAnimation()
    {
        float speed = _stateMachine.Movement.CurrentSpeed;
        SetFloat(AnimatorParams.Speed, speed);
    }
}
