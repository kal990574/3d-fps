using UnityEngine;

public class PlayerGunFire : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Gun _gun;
    [SerializeField] private Transform _fireTransform;
    [SerializeField] private ParticleSystem _hitEffect;
    [SerializeField] private Recoil _recoil;
    [SerializeField] private CameraRotate _cameraRotate;
    [SerializeField] private CameraShake _cameraShake;

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask _hitLayers;
    [SerializeField] private float _maxRayDistance = 1000f;

    [Header("Shake Settings")]
    [SerializeField] private float _fireTrauma = 0.2f;

    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleFireInput();
        HandleReloadInput();
    }

    private void HandleFireInput()
    {
        if (Input.GetMouseButton(0) && _gun.CanFire())
        {
            _gun.Fire();
            PerformRaycast();
            ApplyRecoil();
            ApplyShake();
        }
    }

    private void ApplyRecoil()
    {
        Vector2 recoil = _recoil.GetRecoil();
        _cameraRotate.ApplyRecoil(recoil.x, recoil.y);
    }

    private void ApplyShake()
    {
        _cameraShake.AddTrauma(_fireTrauma);
    }

    private void HandleReloadInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _gun.StartReload();
        }
    }

    private void PerformRaycast()
    {
        Vector3 targetPoint = GetCrosshairTargetPoint();
        FireToTarget(targetPoint);
    }

    private Vector3 GetCrosshairTargetPoint()
    {
        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, _maxRayDistance, _hitLayers, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawLine(_mainCamera.transform.position, hit.point, Color.blue, 0.5f);
            return hit.point;
        }

        return ray.origin + ray.direction * _maxRayDistance;
    }

    private void FireToTarget(Vector3 targetPoint)
    {
        Vector3 fireDirection = (targetPoint - _fireTransform.position).normalized;
        float maxDistance = Vector3.Distance(_fireTransform.position, targetPoint) + 1f;

        Ray ray = new Ray(_fireTransform.position, fireDirection);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, _hitLayers, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawLine(_fireTransform.position, hit.point, Color.red, 0.5f);
            PlayHitEffect(hit);
            ApplyDamage(hit);
        }
        else
        {
            Debug.DrawLine(_fireTransform.position, targetPoint, Color.gray, 0.5f);
        }
    }

    private void ApplyDamage(RaycastHit hit)
    {
        IDamageable damageable = hit.collider.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(_gun.Damage);
        }
    }

    private void PlayHitEffect(RaycastHit hitInfo)
    {
        if (_hitEffect == null)
        {
            return;
        }

        _hitEffect.transform.position = hitInfo.point;
        _hitEffect.transform.forward = hitInfo.normal;
        _hitEffect.Play();
    }
}
