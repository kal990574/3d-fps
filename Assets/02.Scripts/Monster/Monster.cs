using System;
using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour, IDamageable
{
    private const float PatrolSpeedMultiplier = 0.5f;
    private const float PatrolArrivalThreshold = 1f;
    private const float RotationSpeed = 5f;

    public EMonsterState State { get; private set; } = EMonsterState.Idle;
    public float Health { get; private set; } = 200f;

    public event Action<float, float> OnHealthChanged;

    [SerializeField] private GameObject _player;
    [SerializeField] private CharacterController _controller;

    [Header("Detection")]
    public float DetectDistance = 4f;
    public float AttackDistance = 1.2f;
    public float MaxTraceDistance = 15f;

    [Header("Movement")]
    public float MoveSpeed = 5f;

    [Header("Attack")]
    public float AttackSpeed = 2f;
    public float AttackDamage = 10f;

    [Header("Patrol")]
    public float PatrolRadius = 5f;
    public float PatrolIdleTime = 2f;

    [Header("Comeback")]
    public float ComebackStopDistance = 0.5f;

    [Header("Knockback")]
    public float KnockbackDistance = 2f;
    public float KnockbackDuration = 0.3f;

    private Vector3 _originPosition;
    private Vector3 _patrolTarget;
    private float _patrolTimer;
    private float _attackTimer;

    private Transform _playerTransform;
    private IDamageable _playerDamageable;

    private void Start()
    {
        InitializeReferences();
        InitializePosition();
        OnHealthChanged?.Invoke(Health, 200f);
    }

    private void Update()
    {
        switch (State)
        {
            case EMonsterState.Idle:
                Idle();
                break;
            case EMonsterState.Patrol:
                Patrol();
                break;
            case EMonsterState.Trace:
                Trace();
                break;
            case EMonsterState.Comeback:
                Comeback();
                break;
            case EMonsterState.Attack:
                Attack();
                break;
        }
    }

    private void InitializeReferences()
    {
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }

        if (_controller == null)
        {
            _controller = GetComponent<CharacterController>();
        }

        _playerTransform = _player.transform;
        _playerDamageable = _player.GetComponent<IDamageable>();
    }

    private void InitializePosition()
    {
        _originPosition = transform.position;
        _patrolTarget = _originPosition;
    }

    private void Idle()
    {
        if (IsPlayerInRange(DetectDistance))
        {
            TransitionToState(EMonsterState.Trace);
            return;
        }

        _patrolTimer += Time.deltaTime;
        if (_patrolTimer >= PatrolIdleTime)
        {
            _patrolTimer = 0f;
            SetRandomPatrolTarget();
            TransitionToState(EMonsterState.Patrol);
        }
    }

    private void Patrol()
    {
        if (IsPlayerInRange(DetectDistance))
        {
            TransitionToState(EMonsterState.Trace);
            return;
        }

        MoveTowards(_patrolTarget, PatrolSpeedMultiplier);
        LookAtTarget(_patrolTarget);

        if (HasReachedPatrolTarget())
        {
            _patrolTimer = 0f;
            TransitionToState(EMonsterState.Idle);
        }
    }

    private void Trace()
    {
        float distance = GetDistanceToPlayer();

        if (distance > MaxTraceDistance)
        {
            TransitionToState(EMonsterState.Comeback);
            return;
        }

        if (distance <= AttackDistance)
        {
            TransitionToState(EMonsterState.Attack);
            return;
        }

        MoveTowards(_playerTransform.position);
        LookAtTarget(_playerTransform.position);
    }

    private void Comeback()
    {
        MoveTowards(_originPosition);
        LookAtTarget(_originPosition);

        if (Vector3.Distance(transform.position, _originPosition) < ComebackStopDistance)
        {
            _patrolTimer = 0f;
            TransitionToState(EMonsterState.Idle);
        }
    }

    private void Attack()
    {
        if (GetDistanceToPlayer() > AttackDistance)
        {
            TransitionToState(EMonsterState.Trace);
            return;
        }

        LookAtTarget(_playerTransform.position);

        _attackTimer += Time.deltaTime;
        if (_attackTimer >= AttackSpeed)
        {
            _attackTimer = 0f;
            DealDamageToPlayer();
        }
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (State == EMonsterState.Death)
        {
            return;
        }

        Health -= damageInfo.Damage;
        OnHealthChanged?.Invoke(Health, 200f);

        if (Health > 0)
        {
            if (State != EMonsterState.Hit && damageInfo.ApplyKnockback)
            {
                TransitionToState(EMonsterState.Hit);
                StartCoroutine(Hit_Coroutine(damageInfo.SourcePosition, damageInfo.KnockbackStrength));
            }
        }
        else
        {
            TransitionToState(EMonsterState.Death);
            StopAllCoroutines();
            StartCoroutine(Death_Coroutine());
        }
    }

    private IEnumerator Hit_Coroutine(Vector3 damageSourcePosition, float knockbackStrength)
    {
        Vector3 knockbackDirection = CalculateKnockbackDirection(damageSourcePosition);
        float effectiveDistance = KnockbackDistance * knockbackStrength;
        float knockbackSpeed = effectiveDistance / KnockbackDuration;

        float elapsed = 0f;
        while (elapsed < KnockbackDuration)
        {
            float progress = elapsed / KnockbackDuration;
            float strength = Mathf.Lerp(1f, 0f, progress);

            _controller.Move(knockbackDirection * (knockbackSpeed * strength * Time.deltaTime));

            elapsed += Time.deltaTime;
            yield return null;
        }

        TransitionToState(EMonsterState.Idle);
    }

    private IEnumerator Death_Coroutine()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    private bool IsPlayerInRange(float range)
    {
        return GetDistanceToPlayer() <= range;
    }

    private float GetDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, _playerTransform.position);
    }

    private float GetHorizontalDistance(Vector3 from, Vector3 to)
    {
        float dx = to.x - from.x;
        float dz = to.z - from.z;
        return Mathf.Sqrt(dx * dx + dz * dz);
    }

    private bool HasReachedPatrolTarget()
    {
        return GetHorizontalDistance(transform.position, _patrolTarget) < PatrolArrivalThreshold;
    }

    private void MoveTowards(Vector3 target, float speedMultiplier = 1f)
    {
        Vector3 direction = CalculateDirectionToTarget(target);
        _controller.Move(direction * (MoveSpeed * speedMultiplier * Time.deltaTime));
    }

    private Vector3 CalculateDirectionToTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0;
        return direction;
    }

    private Vector3 CalculateKnockbackDirection(Vector3 damageSourcePosition)
    {
        Vector3 direction = (transform.position - damageSourcePosition).normalized;
        direction.y = 0;
        return direction;
    }

    private void LookAtTarget(Vector3 targetPosition)
    {
        Vector3 direction = CalculateDirectionToTarget(targetPosition);

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        }
    }

    private void SetRandomPatrolTarget()
    {
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * PatrolRadius;
        _patrolTarget = _originPosition + new Vector3(randomCircle.x, 0, randomCircle.y);
    }

    private void DealDamageToPlayer()
    {
        if (_playerDamageable != null)
        {
            DamageInfo damageInfo = new DamageInfo(AttackDamage, transform.position, EDamageType.Melee);
            _playerDamageable.TakeDamage(damageInfo);
        }
    }

    private void TransitionToState(EMonsterState newState)
    {
        State = newState;
    }
}