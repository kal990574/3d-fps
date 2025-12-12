using UnityEngine;
using UnityEngine.UI;

public class StatUI : MonoBehaviour
{
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _staminaSlider;

    private void OnEnable()
    {
        GameEvents.OnHealthChanged += UpdateHealth;
        GameEvents.OnStaminaChanged += UpdateStamina;
    }

    private void OnDisable()
    {
        GameEvents.OnHealthChanged -= UpdateHealth;
        GameEvents.OnStaminaChanged -= UpdateStamina;
    }

    private void UpdateHealth(float current, float max)
    {
        if (_healthSlider != null)
        {
            _healthSlider.maxValue = max;
            _healthSlider.value = current;
        }
    }

    private void UpdateStamina(float current, float max)
    {
        if (_staminaSlider != null)
        {
            _staminaSlider.maxValue = max;
            _staminaSlider.value = current;
        }
    }
}