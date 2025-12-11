using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Fire Settings")]
    [SerializeField] private float _fireCooldown = 0.1f;
    [SerializeField] private float _damage = 25f;

    [Header("References")]
    [SerializeField] private Magazine _magazine;

    private float _nextFireTime;

    public Magazine Magazine => _magazine;
    public float Damage => _damage;

    public bool CanFire()
    {
        return !_magazine.IsReloading
            && _magazine.HasAmmo()
            && Time.time >= _nextFireTime;
    }

    public void Fire()
    {
        _magazine.ConsumeAmmo();
        _nextFireTime = Time.time + _fireCooldown;
    }

    public void StartReload()
    {
        _magazine.StartReload();
    }
}
