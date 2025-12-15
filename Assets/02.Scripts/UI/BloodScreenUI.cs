using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BloodScreenUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image _bloodImage;

    [Header("Settings")]
    [SerializeField] private float _fadeDuration = 1f;

    private void Start()
    {
        if (_bloodImage != null)
        {
            SetAlpha(0f);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDamaged += ShowBloodEffect;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDamaged -= ShowBloodEffect;
    }

    private void ShowBloodEffect()
    {
        if (_bloodImage == null)
        {
            return;
        }

        _bloodImage.DOKill();
        SetAlpha(0f);

        _bloodImage.DOFade(1f, _fadeDuration * 0.5f)
            .OnComplete(() => _bloodImage.DOFade(0f, _fadeDuration * 0.5f));
    }

    private void SetAlpha(float alpha)
    {
        Color color = _bloodImage.color;
        color.a = alpha;
        _bloodImage.color = color;
    }
}