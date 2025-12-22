using UnityEngine;
using System;

public class EliteMonsterCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float _attackDamage = 50f;
    [SerializeField] private float _attackDistance = 4f;
    [SerializeField] private float _attackRadius = 5f;
    [SerializeField] private float _knockbackStrength = 3f;
    [SerializeField] private LayerMask _targetLayers;

    private Transform _playerTransform;
    private EliteMonsterAnimationController _animController;

    public float AttackDistance => _attackDistance;
    public float AttackRadius => _attackRadius;

    public event Action OnAttackComplete;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }

        _animController = GetComponent<EliteMonsterAnimationController>();
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

    public void TriggerAttack()
    {
        _animController?.TriggerAttackAnimation();
    }

    public void ExecuteAttack()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _attackRadius, _targetLayers);

        foreach (Collider hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                float damagePercent = 1 - (distance / _attackRadius);
                float finalDamage = _attackDamage * Mathf.Clamp01(damagePercent);

                float knockback = Mathf.Lerp(_knockbackStrength, _knockbackStrength * 0.5f, distance / _attackRadius);

                DamageInfo damageInfo = new DamageInfo(finalDamage, transform.position, EDamageType.Melee, knockback);
                damageable.TakeDamage(damageInfo);
            }
        }

        OnAttackComplete?.Invoke();
    }

    public Vector3 GetPlayerPosition()
    {
        return _playerTransform != null ? _playerTransform.position : Vector3.zero;
    }
}
