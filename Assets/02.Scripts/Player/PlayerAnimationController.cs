using UnityEngine;
using System;

public class PlayerAnimationController : BaseAnimationController
{
    private PlayerMove _playerMove;
    private bool _isDead;

    private const float MAX_SPEED = 12f;

    public event Action OnJumpExecute;
    public event Action OnThrowExecute;

    protected override void Awake()
    {
        base.Awake();
        _playerMove = GetComponent<PlayerMove>();
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDeath += OnPlayerDeath;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDeath -= OnPlayerDeath;
    }

    private void Update()
    {
        if (_isDead || _playerMove == null)
        {
            return;
        }

        float normalizedSpeed = _playerMove.CurrentSpeed / MAX_SPEED;
        SetFloat(AnimatorParams.Speed, normalizedSpeed);
    }

    private void OnPlayerDeath()
    {
        _isDead = true;
        SetBool(AnimatorParams.IsDead, true);
    }

    public void TriggerJump()
    {
        SetTrigger(AnimatorParams.Jump);
    }

    public void TriggerShot()
    {
        SetTrigger(AnimatorParams.Shot);
    }

    public void TriggerThrow()
    {
        SetTrigger(AnimatorParams.Throw);
    }

    public void AnimEvent_Jump()
    {
        OnJumpExecute?.Invoke();
    }

    public void AnimEvent_Throw()
    {
        OnThrowExecute?.Invoke();
    }
}