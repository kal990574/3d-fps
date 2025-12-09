using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _staminaSlider;
    [SerializeField] private PlayerMove _playerMove;

    private void Start()
    {
        // PlayerMove를 자동으로 찾기 (할당 안 했을 경우)
        if (_playerMove == null)
        {
            _playerMove = FindObjectOfType<PlayerMove>();
        }

        // Slider 초기화
        if (_playerMove != null)
        {
            if (_healthSlider != null)
            {
                _healthSlider.maxValue = _playerMove.MaxHealth;
                _healthSlider.value = _playerMove.MaxHealth;
            }

            if (_staminaSlider != null)
            {
                _staminaSlider.maxValue = _playerMove.MaxStamina;
                _staminaSlider.value = _playerMove.MaxStamina;
            }
        }
    }

    private void Update()
    {
        // 체력과 스태미나 값을 Slider에 반영
        if (_playerMove != null)
        {
            if (_healthSlider != null)
            {
                _healthSlider.value = _playerMove.CurrentHealth;
            }

            if (_staminaSlider != null)
            {
                _staminaSlider.value = _playerMove.CurrentStamina;
            }
        }
    }
}