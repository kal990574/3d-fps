using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public float MaxHealth = 100f;
    private float _currentHealth;
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
            return true;
        }
        return false;
    }

    public void UseStamina(float amount, float deltaTime)
    {
        _currentStamina -= amount * deltaTime;
        _currentStamina = Mathf.Max(_currentStamina, 0);
    }
    
    public void RecoverStamina(float deltaTime)
    {
        _currentStamina += StaminaRecoveryRate * deltaTime;
        _currentStamina = Mathf.Min(_currentStamina, MaxStamina);
    }


    public bool HasStamina(float amount)
    {
        return _currentStamina >= amount;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Max(_currentHealth, 0);

        if (_currentHealth <= 0)
        {
            Debug.Log("플레이어 사망!");
        }
    }
}