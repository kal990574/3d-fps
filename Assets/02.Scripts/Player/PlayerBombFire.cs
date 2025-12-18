using UnityEngine;

public class PlayerBombFire : MonoBehaviour
{
    [Header("Fire Settings")]
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _throwForce = 15f;

    [Header("Cooldown")]
    [SerializeField] private float _fireCooldown = 0.5f;

    private float _nextFireTime;
    private Camera _mainCamera;
    private PlayerAnimationController _animController;

    private bool _isThrowPending;
    private Vector3 _pendingDirection;

    private void Start()
    {
        CacheMainCamera();
        CacheAnimController();
        ValidateReferences();
    }

    private void CacheMainCamera()
    {
        _mainCamera = Camera.main;
    }

    private void CacheAnimController()
    {
        _animController = GetComponent<PlayerAnimationController>();

        if (_animController != null)
        {
            _animController.OnThrowExecute += ExecuteThrow;
        }
    }

    private void OnDestroy()
    {
        if (_animController != null)
        {
            _animController.OnThrowExecute -= ExecuteThrow;
        }
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
    }

    private void Update()
    {
        HandleFireInput();
    }

    private void HandleFireInput()
    {
        if (InputManager.Instance.BombPressed && CanFire())
        {
            Fire();
        }
    }

    private bool CanFire()
    {
        return Time.time >= _nextFireTime;
    }

    private void Fire()
    {
        _isThrowPending = true;
        _pendingDirection = _mainCamera.transform.forward;

        ApplyCooldown();
        _animController?.TriggerThrow();
    }

    private void ExecuteThrow()
    {
        if (!_isThrowPending)
        {
            return;
        }

        Bomb bomb = GameplayPoolManager.Instance?.GetBomb(_firePoint.position);

        if (bomb != null)
        {
            bomb.Launch(_pendingDirection, _throwForce);
        }

        _isThrowPending = false;
    }

    private void ApplyCooldown()
    {
        _nextFireTime = Time.time + _fireCooldown;
    }
}
