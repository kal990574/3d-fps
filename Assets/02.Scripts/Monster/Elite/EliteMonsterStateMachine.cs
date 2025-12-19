using UnityEngine;

public class EliteMonsterStateMachine : MonoBehaviour
{
    public EEliteMonsterState CurrentStateType { get; private set; } = EEliteMonsterState.Idle;

    [Header("Detection Settings")]
    [SerializeField] private float _detectDistance = 20f;

    [Header("Attack Settings")]
    [SerializeField] private float _attackCooldown = 3f;

    private IMonsterState _currentState;
    private MonsterHealth _health;
    private EliteMonsterMovement _movement;
    private EliteMonsterCombat _combat;

    private GameObject _player;
    private Transform _playerTransform;

    private float _lastAttackTime = -100f;

    public float AttackCooldown => _attackCooldown;

    public float DetectDistance => _detectDistance;



    public MonsterHealth Health => _health;
    public EliteMonsterMovement Movement => _movement;
    public EliteMonsterCombat Combat => _combat;
    public Transform PlayerTransform => _playerTransform;

    private void Awake()
    {
        _health = GetComponent<MonsterHealth>();
        _movement = GetComponent<EliteMonsterMovement>();
        _combat = GetComponent<EliteMonsterCombat>();
    }

    private void Start()
    {
        InitializeReferences();
        ChangeState(EEliteMonsterState.Idle);
    }

    private void Update()
    {
        _currentState?.Execute();
    }

    private void InitializeReferences()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player != null)
        {
            _playerTransform = _player.transform;
        }
    }

    public void ChangeState(IMonsterState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    public void ChangeState(EEliteMonsterState stateType)
    {
        EEliteMonsterState previousState = CurrentStateType;
        CurrentStateType = stateType;

        IMonsterState newState = stateType switch
        {
            EEliteMonsterState.Idle => new EliteIdleState(this),
            EEliteMonsterState.Trace => new EliteTraceState(this),
            EEliteMonsterState.Attack => new EliteAttackState(this),
            EEliteMonsterState.Death => new EliteDeadState(this),
            _ => new EliteIdleState(this)
        };

        ChangeState(newState);
    }

    public bool IsPlayerInRange(float range)
    {
        if (_playerTransform == null)
        {
            return false;
        }

        return _movement.GetDistanceTo(_playerTransform.position) <= range;
    }

    public float GetDistanceToPlayer()
    {
        if (_playerTransform == null)
        {
            return float.MaxValue;
        }

        return _movement.GetDistanceTo(_playerTransform.position);
    }

    public bool CanAttack()
    {
        return Time.time - _lastAttackTime >= _attackCooldown;
    }

    public void RecordAttackUsed()
    {
        _lastAttackTime = Time.time;
    }


}
