using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GunUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Gun _gun;

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
        _gun.OnAmmoChanged += UpdateAmmoText;
        _gun.OnReloadStart += ShowReloadUI;
        _gun.OnReloadComplete += HideReloadUI;
    }

    private void UnsubscribeEvents()
    {
        _gun.OnAmmoChanged -= UpdateAmmoText;
        _gun.OnReloadStart -= ShowReloadUI;
        _gun.OnReloadComplete -= HideReloadUI;
    }

    private void UpdateAmmoText()
    {
        _ammoText.text = $"{_gun.CurrentAmmo} / {_gun.ReserveAmmo}";
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
        if (_gun.IsReloading)
        {
            _reloadSlider.value = _gun.ReloadProgress;
        }
    }
}
