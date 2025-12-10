using UnityEngine;
using TMPro;

public class BombUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _bombCountText;

    [Header("Reference")]
    [SerializeField] private BombPool _bombPool;

    private void Start()
    {
        if (_bombPool == null)
        {
            _bombPool = FindObjectOfType<BombPool>();
        }

        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (_bombPool == null || _bombCountText == null)
        {
            return;
        }

        _bombCountText.text = $"{_bombPool.AvailableCount} / {_bombPool.MaxPoolSize}";
    }
}
