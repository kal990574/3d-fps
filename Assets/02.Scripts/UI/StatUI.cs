using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StatUI : MonoBehaviour
{
    [Header("Main Sliders")]
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _staminaSlider;

    [Header("Health Impact Effects")]
    [SerializeField] private Slider _healthDelaySlider;
    [SerializeField] private Image _healthFlashOverlay;
    [SerializeField] private RectTransform _healthShakeTarget;

    [Header("Impact Settings")]
    [SerializeField] private float _delayDuration = 0.5f;
    [SerializeField] private float _flashDuration = 0.1f;
    [SerializeField] private float _shakeDuration = 0.15f;
    [SerializeField] private float _shakeStrength = 5f;

    private float _previousHealthValue = -1f;

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

    private void Start()
    {
        if (_healthFlashOverlay != null)
        {
            _healthFlashOverlay.color = new Color(1f, 1f, 1f, 0f);
        }
    }

    private void UpdateHealth(float current, float max)
    {
        if (_healthSlider != null)
        {
            _healthSlider.maxValue = max;
            _healthSlider.value = current;
        }

        if (_healthDelaySlider != null)
        {
            _healthDelaySlider.maxValue = max;
            
            // 첫 호출 시.
            if (_previousHealthValue < 0f)
            {
                _healthDelaySlider.value = current;
                _previousHealthValue = current;
                return;
            }

            bool tookDamage = current < _previousHealthValue;

            if (tookDamage)
            {
                PlayHealthHitEffects();
                PlayDelayBarAnimation(current);
            }
            else
            {
                _healthDelaySlider.DOKill();
                _healthDelaySlider.value = current;
            }
        }

        _previousHealthValue = current;
    }

    private void PlayDelayBarAnimation(float targetValue)
    {
        if (_healthDelaySlider == null)
        {
            return;
        }

        DOTween.Kill(_healthDelaySlider);
        DOTween.To(
            () => _healthDelaySlider.value,
            x => _healthDelaySlider.value = x,
            targetValue,
            _delayDuration
        ).SetEase(Ease.OutQuad).SetTarget(_healthDelaySlider);
    }

    private void PlayHealthHitEffects()
    {
        PlayFlashEffect();
        PlayShakeEffect();
    }

    private void PlayFlashEffect()
    {
        if (_healthFlashOverlay == null)
        {
            return;
        }

        _healthFlashOverlay.DOKill();
        _healthFlashOverlay.color = new Color(1f, 1f, 1f, 1f);
        _healthFlashOverlay.DOFade(0f, _flashDuration);
    }

    private void PlayShakeEffect()
    {
        if (_healthShakeTarget == null)
        {
            return;
        }

        _healthShakeTarget.DOKill();
        _healthShakeTarget.DOShakeAnchorPos(_shakeDuration, _shakeStrength, 20, 90f, false, false);
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