public interface IWeapon
{
    string WeaponName { get; }
    bool CanFire { get; }
    float Damage { get; }

    void Fire();
    void Reload();
}
