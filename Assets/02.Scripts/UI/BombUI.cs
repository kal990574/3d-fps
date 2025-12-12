using UnityEngine;
using TMPro;

public class BombUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _bombCountText;

    private void OnEnable()
    {
        GameEvents.OnBombCountChanged += UpdateBombCount;
    }

    private void OnDisable()
    {
        GameEvents.OnBombCountChanged -= UpdateBombCount;
    }

    private void UpdateBombCount(int available, int max)
    {
        if (_bombCountText != null)
        {
            _bombCountText.text = $"{available} / {max}";
        }
    }
}