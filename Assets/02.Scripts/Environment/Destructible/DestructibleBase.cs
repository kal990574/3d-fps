using UnityEngine;

public abstract class DestructibleBase : MonoBehaviour, IDamageable, IDestructible
{
    [Header("Health")]
    [SerializeField] protected float _maxHealth = 100f;

    [Header("Destruction")]
    [SerializeField] protected float _destroyDelay = 5f;

    protected float _currentHealth;
    protected EDestructibleState _state = EDestructibleState.Intact;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;
    public bool IsDestroyed => _state == EDestructibleState.Destroyed;

    protected virtual void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public virtual void TakeDamage(DamageInfo damageInfo)
    {
        if (IsDestroyed)
        {
            return;
        }

        _currentHealth -= damageInfo.Damage;

        if (_currentHealth <= 0)
        {
            Destruct();
        }
    }

    protected virtual void Destruct()
    {
        if (IsDestroyed)
        {
            return;
        }

        _state = EDestructibleState.Destroyed;
        OnDestruct();

        if (_destroyDelay > 0)
        {
            Destroy(gameObject, _destroyDelay);
        }
    }

    public abstract void OnDestruct();
}