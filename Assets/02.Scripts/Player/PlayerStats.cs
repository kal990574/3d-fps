using UnityEngine;
using System;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public float MaxHealth = 100f;
    [SerializeField] private float _currentHealth;
    public float CurrentHealth => _currentHealth;

    [Header("Stamina")]
    public float MaxStamina = 100f;
    public float StaminaRecoveryRate = 10f; // 초당 회복량
    private float _currentStamina;
    public float CurrentStamina => _currentStamina;

    private void Start()
    {
        _currentHealth = MaxHealth;
        _currentStamina = MaxStamina;

        GameEvents.TriggerHealthChanged(_currentHealth, MaxHealth);
        GameEvents.TriggerStaminaChanged(_currentStamina, MaxStamina);
    }

    private void Update()
    {
        // 자동 스태미나 회복
        RecoverStamina(Time.deltaTime);
    }

    public bool TryUseStamina(float amount)
    {
        if (_currentStamina >= amount)
        {
            _currentStamina -= amount;
            _currentStamina = Mathf.Max(_currentStamina, 0);
            GameEvents.TriggerStaminaChanged(_currentStamina, MaxStamina);
            return true;
        }
        return false;
    }

    public void UseStamina(float amount, float deltaTime)
    {
        float prevStamina = _currentStamina;
        _currentStamina -= amount * deltaTime;
        _currentStamina = Mathf.Max(_currentStamina, 0);

        if (Mathf.Abs(prevStamina - _currentStamina) > 0.01f)
        {
            GameEvents.TriggerStaminaChanged(_currentStamina, MaxStamina);
        }
    }

    public void RecoverStamina(float deltaTime)
    {
        float prevStamina = _currentStamina;
        _currentStamina += StaminaRecoveryRate * deltaTime;
        _currentStamina = Mathf.Min(_currentStamina, MaxStamina);

        if (Mathf.Abs(prevStamina - _currentStamina) > 0.01f)
        {
            GameEvents.TriggerStaminaChanged(_currentStamina, MaxStamina);
        }
    }


    public bool HasStamina(float amount)
    {
        return _currentStamina >= amount;
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        _currentHealth -= damageInfo.Damage;
        _currentHealth = Mathf.Max(_currentHealth, 0);

        GameEvents.TriggerHealthChanged(_currentHealth, MaxHealth);
        GameEvents.TriggerPlayerDamaged();

        if (_currentHealth <= 0)
        {
            GameEvents.TriggerPlayerDeath();
        }
    }
}