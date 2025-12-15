using UnityEngine;
using UnityEngine.UI;

public class MonsterHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MonsterHealth _monsterHealth;
    [SerializeField] private Image _healthBarFill;

    [Header("Settings")]
    [SerializeField] private Color _healthBarColor = Color.red;

    private Camera _mainCamera;

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
    }

    private void InitializeUI()
    {
        if (_healthBarFill != null)
        {
            _healthBarFill.color = _healthBarColor;
            _healthBarFill.type = Image.Type.Filled;
            _healthBarFill.fillMethod = Image.FillMethod.Horizontal;
        }
    }

    private void UpdateHealth(float current, float max)
    {
        if (_healthBarFill != null)
        {
            float fillAmount = Mathf.Clamp01(current / max);
            _healthBarFill.fillAmount = fillAmount;
        }
    }

    private void ApplyBillboardEffect()
    {
        if (_mainCamera != null)
        {
            transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward, _mainCamera.transform.rotation * Vector3.up);
        }
    }
}