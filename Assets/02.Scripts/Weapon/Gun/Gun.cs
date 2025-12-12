using UnityEngine;

public abstract class Gun : MonoBehaviour, IWeapon
{
    [Header("Weapon Info")]
    [SerializeField] protected string _weaponName = "Gun";

    [Header("Fire Settings")]
    [SerializeField] protected float _fireCooldown = 0.1f;
    [SerializeField] protected float _damage = 25f;

    [Header("References")]
    [SerializeField] protected Magazine _magazine;

    protected float _nextFireTime;

    public string WeaponName => _weaponName;
    public float Damage => _damage;
    public virtual bool CanFire => !_magazine.IsReloading && _magazine.HasAmmo() && Time.time >= _nextFireTime;

    public Magazine Magazine => _magazine;

    public virtual void Fire()
    {
        if (!CanFire) return;

        _magazine.ConsumeAmmo();
        _nextFireTime = Time.time + _fireCooldown;
        OnFire();
    }

    public virtual void Reload()
    {
        _magazine.StartReload();
    }

    protected virtual void OnFire()
    {
        
    }
}