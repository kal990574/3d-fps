using UnityEngine;

public class PlayerBombFire : MonoBehaviour
{
    [Header("Bomb Settings")]
    [SerializeField] private GameObject _bombPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _throwForce = 15f;

    [Header("Cooldown")]
    [SerializeField] private float _fireCooldown = 0.5f;
    private float _nextFireTime = 0f;

    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;

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
        if (Input.GetMouseButtonDown(0) && CanFire())
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
        if (_bombPrefab == null)
        {
            Debug.LogError("BombPrefab is not assigned!");
            return;
        }

        Bomb bomb = CreateBomb();
        Vector3 fireDirection = _mainCamera.transform.forward;
        bomb.Launch(fireDirection, _throwForce);
        ApplyCooldown();
    }

    private Bomb CreateBomb()
    {
        Vector3 firePosition = _firePoint.position;
        GameObject bombObject = Instantiate(_bombPrefab, firePosition, Quaternion.identity);
        return bombObject.GetComponent<Bomb>();
    }

    private void ApplyCooldown()
    {
        _nextFireTime = Time.time + _fireCooldown;
    }
}
