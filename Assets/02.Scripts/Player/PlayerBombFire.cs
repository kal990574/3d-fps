using UnityEngine;

public class PlayerBombFire : MonoBehaviour
{
    [Header("Pool Reference")]
    [SerializeField] private BombPool _bombPool;

    [Header("Fire Settings")]
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _throwForce = 15f;

    [Header("Cooldown")]
    [SerializeField] private float _fireCooldown = 0.5f;

    private float _nextFireTime;
    private Camera _mainCamera;

    private void Start()
    {
        CacheMainCamera();
        ValidateReferences();
    }

    private void CacheMainCamera()
    {
        _mainCamera = Camera.main;
    }

    private void ValidateReferences()
    {
        if (_mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
        }

        if (_firePoint == null)
        {
            Debug.LogError("FirePoint not assigned!");
        }

        if (_bombPool == null)
        {
            Debug.LogError("BombPool not assigned!");
        }
    }

    private void Update()
    {
        HandleFireInput();
    }

    private void HandleFireInput()
    {
        if (Input.GetMouseButtonDown(1) && CanFire())
        {
            Fire();
        }
    }

    private bool CanFire()
    {
        return Time.time >= _nextFireTime && _bombPool.CanGet;
    }

    private void Fire()
    {
        Bomb bomb = _bombPool.Get(_firePoint.position);

        if (bomb == null)
        {
            return;
        }

        Vector3 fireDirection = _mainCamera.transform.forward;
        bomb.Launch(fireDirection, _throwForce);
        ApplyCooldown();
    }

    private void ApplyCooldown()
    {
        _nextFireTime = Time.time + _fireCooldown;
    }
}
