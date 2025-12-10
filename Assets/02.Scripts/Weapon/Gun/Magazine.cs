using System;
using UnityEngine;

public class Magazine : MonoBehaviour
{
    [Header("Capacity Settings")]
    [SerializeField] private int _capacity = 30;
    [SerializeField] private int _startingMagazines = 5;

    [Header("Reload Settings")]
    [SerializeField] private float _reloadTime = 1.6f;

    private int _currentAmmo;
    private int _reserveAmmo;
    private bool _isReloading;
    private float _reloadTimer;

    public int CurrentAmmo => _currentAmmo;
    public int ReserveAmmo => _reserveAmmo;
    public int Capacity => _capacity;
    public bool IsReloading => _isReloading;
    public float ReloadProgress => _isReloading ? _reloadTimer / _reloadTime : 0f;

    public event Action OnAmmoChanged;
    public event Action OnReloadStart;
    public event Action OnReloadComplete;

    private void Awake()
    {
        InitializeAmmo();
    }

    private void InitializeAmmo()
    {
        _currentAmmo = _capacity;
        _reserveAmmo = _capacity * (_startingMagazines - 1);
    }

    private void Update()
    {
        UpdateReload();
    }

    public bool HasAmmo()
    {
        return _currentAmmo > 0;
    }

    public void ConsumeAmmo()
    {
        if (_currentAmmo <= 0)
        {
            return;
        }

        _currentAmmo--;
        OnAmmoChanged?.Invoke();
    }

    public bool CanReload()
    {
        return !_isReloading
            && _currentAmmo < _capacity
            && _reserveAmmo > 0;
    }

    public void StartReload()
    {
        if (!CanReload())
        {
            return;
        }

        _isReloading = true;
        _reloadTimer = 0f;
        OnReloadStart?.Invoke();
    }

    private void UpdateReload()
    {
        if (!_isReloading)
        {
            return;
        }

        _reloadTimer += Time.deltaTime;

        if (_reloadTimer >= _reloadTime)
        {
            CompleteReload();
        }
    }

    private void CompleteReload()
    {
        int ammoNeeded = _capacity - _currentAmmo;
        int ammoToLoad = Mathf.Min(ammoNeeded, _reserveAmmo);

        _currentAmmo += ammoToLoad;
        _reserveAmmo -= ammoToLoad;
        _isReloading = false;

        OnReloadComplete?.Invoke();
        OnAmmoChanged?.Invoke();
    }
}
