using UnityEngine;

public class MonsterCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float _attackSpeed = 2f;
    [SerializeField] private float _attackDamage = 10f;
    [SerializeField] private float _attackDistance = 1.2f;

    private IDamageable _playerDamageable;
    private Transform _playerTransform;
    private float _attackTimer;

    public float AttackSpeed => _attackSpeed;
    public float AttackDistance => _attackDistance;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
            _playerDamageable = player.GetComponent<IDamageable>();
        }
    }

    public bool CanAttack()
    {
        _attackTimer += Time.deltaTime;

        if (_attackTimer >= _attackSpeed)
        {
            _attackTimer = 0f;
            return true;
        }

        return false;
    }

    public void ResetAttackTimer()
    {
        _attackTimer = _attackSpeed;
    }

    public void DealDamageToPlayer()
    {
        if (_playerDamageable != null && _playerTransform != null)
        {
            DamageInfo damageInfo = new DamageInfo(_attackDamage, transform.position, EDamageType.Melee);
            _playerDamageable.TakeDamage(damageInfo);
        }
    }

    public Vector3 GetPlayerPosition()
    {
        return _playerTransform != null ? _playerTransform.position : Vector3.zero;
    }
}