using UnityEngine;

public class PlayerGunFire : MonoBehaviour
{
    [SerializeField] private Transform _fireTransform;
    [SerializeField] private ParticleSystem _hitEffect;

    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Fire();
        }
    }

    private void Fire()
    {
        Ray ray = new Ray(_fireTransform.position, _mainCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            PlayHitEffect(hitInfo);
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
