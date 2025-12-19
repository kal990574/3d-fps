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
    private MonsterAnimationController _animController;
    private bool _isAttackPending;

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

        _animController = GetComponent<MonsterAnimationController>();
        if (_animController != null)
        {
            _animController.OnAttackExecute += ExecuteAttack;
        }
    }

    private void OnDestroy()
    {
        if (_animController != null)
        {
            _animController.OnAttackExecute -= ExecuteAttack;
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
        _attackTimer = 0;
    }

    public void TriggerAttack()
    {
        _isAttackPending = true;
        _animController?.TriggerAttackAnimation();
    }

    private void ExecuteAttack()
    {
        Debug.Log($"MonsterCombat.ExecuteAttack called, pending: {_isAttackPending}");
        DealDamageToPlayer();
        _isAttackPending = false;
    }

    public void DealDamageToPlayer()
    {
        Debug.Log($"DealDamageToPlayer: player={_playerDamageable}, transform={_playerTransform}");

        if (_playerDamageable != null && _playerTransform != null)
        {
            DamageInfo damageInfo = new DamageInfo(_attackDamage, transform.position, EDamageType.Melee);
            _playerDamageable.TakeDamage(damageInfo);
            Debug.Log($"Damage dealt: {_attackDamage}");
        }
    }

    public Vector3 GetPlayerPosition()
    {
        return _playerTransform != null ? _playerTransform.position : Vector3.zero;
    }
}