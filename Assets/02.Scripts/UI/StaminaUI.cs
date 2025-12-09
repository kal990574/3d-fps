using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _staminaSlider;
    [SerializeField] private PlayerStats _playerStats;

    private void Start()
    {
        // PlayerStats를 자동으로 찾기 (할당 안 했을 경우)
        if (_playerStats == null)
        {
            _playerStats = FindObjectOfType<PlayerStats>();
        }

        // Slider 초기화
        if (_playerStats != null)
        {
            if (_healthSlider != null)
            {
                _healthSlider.maxValue = _playerStats.MaxHealth;
                _healthSlider.value = _playerStats.MaxHealth;
            }

            if (_staminaSlider != null)
            {
                _staminaSlider.maxValue = _playerStats.MaxStamina;
                _staminaSlider.value = _playerStats.MaxStamina;
            }
        }
    }

    private void Update()
    {
        // 체력과 스태미나 값을 Slider에 반영
        if (_playerStats != null)
        {
            if (_healthSlider != null)
            {
                _healthSlider.value = _playerStats.CurrentHealth;
            }

            if (_staminaSlider != null)
            {
                _staminaSlider.value = _playerStats.CurrentStamina;
            }
        }
    }
}