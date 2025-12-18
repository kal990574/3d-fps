using UnityEngine;

public abstract class BaseAnimationController : MonoBehaviour, IAnimatable
{
    protected Animator _animator;

    protected static class AnimatorParams
    {
        public const string Speed = "Speed";
        public const string IsGrounded = "IsGrounded";
        public const string IsDead = "IsDead";
        public const string Attack = "Attack";
        public const string Hit = "Hit";
        public const string Jump = "Jump";
    }

    protected virtual void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void PlayAnimation(string stateName)
    {
        if (_animator != null)
        {
            _animator.Play(stateName);
        }
    }

    public void SetAnimationSpeed(float speed)
    {
        if (_animator != null)
        {
            _animator.speed = speed;
        }
    }

    public void SetFloat(string paramName, float value)
    {
        if (_animator != null)
        {
            _animator.SetFloat(paramName, value);
        }
    }

    public void SetBool(string paramName, bool value)
    {
        if (_animator != null)
        {
            _animator.SetBool(paramName, value);
        }
    }

    public void SetTrigger(string triggerName)
    {
        if (_animator != null)
        {
            _animator.SetTrigger(triggerName);
        }
    }

    protected void ResetTrigger(string triggerName)
    {
        if (_animator != null)
        {
            _animator.ResetTrigger(triggerName);
        }
    }
}
