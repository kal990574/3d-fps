using System;
using System.Collections;
using UnityEngine;

public class MonsterHealth : MonoBehaviour, IDamageable
{
    private const float MaxHealth = 200f;
    private const float DeathDelay = 0.1f;

    public float CurrentHealth { get; private set; } = MaxHealth;
    public bool IsDead => CurrentHealth <= 0;

    public event Action<float, float> OnHealthChanged;
    public event Action<Vector3, float> OnDamaged;
    public event Action OnDeath;
    public event Action OnKnockbackComplete;

    [Header("Knockback Settings")]
    [SerializeField] private float _knockbackDistance = 2f;
    [SerializeField] private float _knockbackDuration = 0.3f;

    private CharacterController _controller;
    private MonsterStateMachine _stateMachine;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _stateMachine = GetComponent<MonsterStateMachine>();
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (IsDead)
        {
            return;
        }

        CurrentHealth -= damageInfo.Damage;
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth > 0)
        {
            OnDamaged?.Invoke(damageInfo.SourcePosition, damageInfo.KnockbackStrength);

            if (damageInfo.ApplyKnockback)
            {
                StartCoroutine(ApplyKnockback_Coroutine(damageInfo.SourcePosition, damageInfo.KnockbackStrength));
            }
        }
        else
        {
            CurrentHealth = 0;
            OnDeath?.Invoke();
            StartCoroutine(Death_Coroutine());
        }
    }

    private IEnumerator ApplyKnockback_Coroutine(Vector3 damageSourcePosition, float knockbackStrength)
    {
        Vector3 knockbackDirection = CalculateKnockbackDirection(damageSourcePosition);
        float effectiveDistance = _knockbackDistance * knockbackStrength;
        float knockbackSpeed = effectiveDistance / _knockbackDuration;

        float elapsed = 0f;
        while (elapsed < _knockbackDuration)
        {
            float progress = elapsed / _knockbackDuration;
            float strength = Mathf.Lerp(1f, 0f, progress);

            _controller.Move(knockbackDirection * (knockbackSpeed * strength * Time.deltaTime));

            elapsed += Time.deltaTime;
            yield return null;
        }

        OnKnockbackComplete?.Invoke();
    }

    private IEnumerator Death_Coroutine()
    {
        yield return new WaitForSeconds(DeathDelay);
        Destroy(gameObject);
    }

    private Vector3 CalculateKnockbackDirection(Vector3 damageSourcePosition)
    {
        Vector3 direction = (transform.position - damageSourcePosition).normalized;
        direction.y = 0;
        return direction;
    }
}