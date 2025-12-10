using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GunUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Magazine _magazine;

    [Header("Ammo UI")]
    [SerializeField] private TextMeshProUGUI _ammoText;

    [Header("Reload UI")]
    [SerializeField] private Slider _reloadSlider;
    [SerializeField] private GameObject _reloadPanel;

    private void Start()
    {
        SubscribeEvents();
        UpdateAmmoText();
        HideReloadUI();
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    private void Update()
    {
        UpdateReloadProgress();
    }

    private void SubscribeEvents()
    {
        _magazine.OnAmmoChanged += UpdateAmmoText;
        _magazine.OnReloadStart += ShowReloadUI;
        _magazine.OnReloadComplete += HideReloadUI;
    }

    private void UnsubscribeEvents()
    {
        _magazine.OnAmmoChanged -= UpdateAmmoText;
        _magazine.OnReloadStart -= ShowReloadUI;
        _magazine.OnReloadComplete -= HideReloadUI;
    }

    private void UpdateAmmoText()
    {
        _ammoText.text = $"{_magazine.CurrentAmmo} / {_magazine.ReserveAmmo}";
    }

    private void ShowReloadUI()
    {
        _reloadPanel.SetActive(true);
        _reloadSlider.value = 0f;
    }

    private void HideReloadUI()
    {
        _reloadPanel.SetActive(false);
    }

    private void UpdateReloadProgress()
    {
        if (_magazine.IsReloading)
        {
            _reloadSlider.value = _magazine.ReloadProgress;
        }
    }
}
