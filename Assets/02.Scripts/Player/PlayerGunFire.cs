using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGunFire : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Gun _gun;
    [SerializeField] private Transform _fireTransform;
    [SerializeField] private ParticleSystem _hitEffect;
    [SerializeField] private CameraRotate _cameraRotate;
    [SerializeField] private CameraShake _cameraShake;
    [SerializeField] private List<GameObject> _muzzleEffects;
    
    private EZoomMode _zoomMode = EZoomMode.Normal;
    [SerializeField] private GameObject _normalCrosshair;
    [SerializeField] private GameObject _zoomInCrosshair;

    [Header("Recoil Settings")]
    [SerializeField] private Recoil _recoil = new Recoil();

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask _hitLayers;
    [SerializeField] private float _maxRayDistance = 1000f;

    [Header("Shake Settings")]
    [SerializeField] private float _fireTrauma = 0.2f;

    private Camera _mainCamera;
    private PlayerAnimationController _animController;

    private void Start()
    {
        _mainCamera = Camera.main;
        _animController = GetComponent<PlayerAnimationController>();
    }

    private void Update()
    {
        HandleFireInput();
        HandleReloadInput();
        ZoomModeCheck();
    }

    private void ZoomModeCheck()
    {
        if (InputManager.Instance.RightClickHeld)
        {
            _zoomMode = EZoomMode.ZoomIn;
            _normalCrosshair.SetActive(false);
            _zoomInCrosshair.SetActive(true);
        }
        else
        {
            _zoomMode = EZoomMode.Normal;
            _normalCrosshair.SetActive(true);
            _zoomInCrosshair.SetActive(false);
        }
    }

    private void HandleFireInput()
    {
        if (InputManager.Instance.FireHeld && _gun.CanFire)
        {
            _gun.Fire();
            PerformRaycast();
            ApplyRecoil();
            ApplyShake();
            _animController?.TriggerShot();
            StartCoroutine(MuzzleFlash_Coroutine());
        }
    }

    private IEnumerator MuzzleFlash_Coroutine()
    {
        GameObject muzzleEffect = _muzzleEffects[Random.Range(0, _muzzleEffects.Count)];
        muzzleEffect.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        muzzleEffect.SetActive(false);
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
        if (InputManager.Instance.ReloadPressed)
        {
            _gun.Reload();
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
            DamageInfo damageInfo = new DamageInfo(_gun.Damage, transform.position, EDamageType.Bullet);
            damageable.TakeDamage(damageInfo);
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
