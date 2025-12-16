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
        yield return new WaitForSeconds(DeathDelay);
        Destroy(gameObject);
    }
}