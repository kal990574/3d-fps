using System;
using System.Collections;
using UnityEngine;

public class MonsterHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float _maxHealth = 200f;
    [SerializeField] private float _deathDelay = 0.1f;

    public float MaxHealth => _maxHealth;
    public float CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0;

    public event Action<float, float> OnHealthChanged;
    public event Action<Vector3, float> OnDamaged;
    public event Action OnDeath;

    private void Awake()
    {
        CurrentHealth = _maxHealth;
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(CurrentHealth, _maxHealth);
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (IsDead)
        {
            return;
        }

        CurrentHealth -= damageInfo.Damage;
        OnHealthChanged?.Invoke(CurrentHealth, _maxHealth);

        if (CurrentHealth > 0)
        {
            float knockbackStrength = damageInfo.ApplyKnockback ? damageInfo.KnockbackStrength : 0f;
            OnDamaged?.Invoke(damageInfo.SourcePosition, knockbackStrength);
        }
        else
        {
            CurrentHealth = 0;
            OnDeath?.Invoke();
            StartCoroutine(DeathCoroutine());
        }
    }

    private IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(_deathDelay);
        Destroy(gameObject);
    }
}