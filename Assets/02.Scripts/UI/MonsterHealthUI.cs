using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MonsterHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MonsterHealth _monsterHealth;
    [SerializeField] private Image _healthBarFill;

    [Header("Settings")] [SerializeField] private Color _healthBarColor;

    [Header("Impact Effects")]
    [SerializeField] private Image _delayBarFill;
    [SerializeField] private Image _flashOverlay;
    [SerializeField] private RectTransform _shakeTarget;
    [SerializeField] private Color _delayBarColor;

    [Header("Delay Settings")]
    [SerializeField] private float _delayDuration = 0.5f;

    [Header("Flash Settings")]
    [SerializeField] private float _flashDuration = 0.1f;

    [Header("Shake Settings")]
    [SerializeField] private float _shakeDuration = 0.15f;
    [SerializeField] private float _shakeStrength = 5f;

    private Camera _mainCamera;
    private float _targetFillAmount = 1f;
    private Vector2 _originalShakePosition;

    private void Start()
    {
        InitializeReferences();
        InitializeUI();
    }

    private void OnEnable()
    {
        if (_monsterHealth != null)
        {
            _monsterHealth.OnHealthChanged += UpdateHealth;
        }
    }

    private void OnDisable()
    {
        if (_monsterHealth != null)
        {
            _monsterHealth.OnHealthChanged -= UpdateHealth;
        }

        KillAllTweens();
    }

    private void KillAllTweens()
    {
        if (_delayBarFill != null)
        {
            DOTween.Kill(_delayBarFill);
        }
        if (_flashOverlay != null)
        {
            DOTween.Kill(_flashOverlay);
        }
        if (_shakeTarget != null)
        {
            DOTween.Kill(_shakeTarget);
        }
    }

    private void LateUpdate()
    {
        ApplyBillboardEffect();
    }

    private void InitializeReferences()
    {
        _mainCamera = Camera.main;

        if (_monsterHealth == null)
        {
            _monsterHealth = GetComponentInParent<MonsterHealth>();
        }

        if (_shakeTarget == null)
        {
            _shakeTarget = GetComponent<RectTransform>();
        }

        if (_shakeTarget != null)
        {
            _originalShakePosition = _shakeTarget.anchoredPosition;
        }
    }

    private void InitializeUI()
    {
        if (_healthBarFill != null)
        {
            _healthBarFill.color = _healthBarColor;
            _healthBarFill.type = Image.Type.Filled;
            _healthBarFill.fillMethod = Image.FillMethod.Horizontal;
        }

        if (_delayBarFill != null)
        {
            _delayBarFill.color = _delayBarColor;
            _delayBarFill.type = Image.Type.Filled;
            _delayBarFill.fillMethod = Image.FillMethod.Horizontal;
        }

        if (_flashOverlay != null)
        {
            _flashOverlay.color = new Color(1f, 1f, 1f, 0f);
        }
    }

    private void UpdateHealth(float current, float max)
    {
        float newFillAmount = Mathf.Clamp01(current / max);
        bool tookDamage = newFillAmount < _targetFillAmount;

        _targetFillAmount = newFillAmount;

        if (_healthBarFill != null)
        {
            _healthBarFill.fillAmount = _targetFillAmount;
        }

        if (tookDamage)
        {
            PlayHitEffects();
            PlayDelayBarAnimation(_targetFillAmount);
        }
        else
        {
            // 회복 시 딜레이 바도 즉시 동기화
            if (_delayBarFill != null)
            {
                DOTween.Kill(_delayBarFill);
                _delayBarFill.fillAmount = _targetFillAmount;
            }
        }
    }

    private void PlayDelayBarAnimation(float targetValue)
    {
        if (_delayBarFill == null)
        {
            return;
        }

        DOTween.Kill(_delayBarFill);
        DOTween.To(
            () => _delayBarFill.fillAmount,
            x => _delayBarFill.fillAmount = x,
            targetValue,
            _delayDuration
        ).SetEase(Ease.OutQuad).SetTarget(_delayBarFill);
    }

    private void PlayHitEffects()
    {
        PlayFlashEffect();
        PlayShakeEffect();
    }

    private void PlayFlashEffect()
    {
        if (_flashOverlay == null)
        {
            return;
        }

        _flashOverlay.DOKill();
        _flashOverlay.color = new Color(1f, 1f, 1f, 1f);
        _flashOverlay.DOFade(0f, _flashDuration);
    }

    private void PlayShakeEffect()
    {
        if (_shakeTarget == null)
        {
            return;
        }

        _shakeTarget.DOKill();
        _shakeTarget.anchoredPosition = _originalShakePosition;
        _shakeTarget.DOShakeAnchorPos(_shakeDuration, _shakeStrength, 20, 90f, false, false);
    }

    private void ApplyBillboardEffect()
    {
        if (_mainCamera != null)
        {
            transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward, _mainCamera.transform.rotation * Vector3.up);
        }
    }
}