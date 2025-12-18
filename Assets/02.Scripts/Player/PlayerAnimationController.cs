using UnityEngine;

public class PlayerAnimationController : BaseAnimationController
{
    private PlayerMove _playerMove;
    private bool _isDead;

    private const float MAX_SPEED = 12f;

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
}