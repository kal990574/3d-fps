using UnityEngine;

public class PlayerStats : MonoBehaviour
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

    /// <summary>
    /// 점프 시 스태미나 사용 시도
    /// </summary>
    /// <param name="amount">사용할 스태미나 양</param>
    /// <returns>사용 성공 여부</returns>
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

    /// <summary>
    /// 달리기 시 스태미나 소모
    /// </summary>
    public void UseStamina(float amount, float deltaTime)
    {
        _currentStamina -= amount * deltaTime;
        _currentStamina = Mathf.Max(_currentStamina, 0);
    }

    /// <summary>
    /// 스태미나 회복
    /// </summary>
    public void RecoverStamina(float deltaTime)
    {
        _currentStamina += StaminaRecoveryRate * deltaTime;
        _currentStamina = Mathf.Min(_currentStamina, MaxStamina);
    }

    /// <summary>
    /// 스태미나가 충분한지 확인
    /// </summary>
    public bool HasStamina(float amount)
    {
        return _currentStamina >= amount;
    }
}