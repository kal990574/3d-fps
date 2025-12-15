using UnityEngine;

public class MonsterStateMachine : MonoBehaviour
{
    public EMonsterState CurrentStateType { get; private set; } = EMonsterState.Idle;

    [Header("Detection Settings")]
    [SerializeField] private float _detectDistance = 4f;
    [SerializeField] private float _maxTraceDistance = 15f;

    [Header("Patrol Settings")]
    [SerializeField] private float _patrolRadius = 5f;
    [SerializeField] private float _patrolIdleTime = 2f;

    [Header("Comeback Settings")]
    [SerializeField] private float _comebackStopDistance = 0.5f;

    private IMonsterState _currentState;
    private MonsterHealth _health;
    private MonsterMovement _movement;
    private MonsterCombat _combat;

    private GameObject _player;
    private Transform _playerTransform;
    private Vector3 _originPosition;

    public float DetectDistance => _detectDistance;
    public float MaxTraceDistance => _maxTraceDistance;
    public float PatrolRadius => _patrolRadius;
    public float PatrolIdleTime => _patrolIdleTime;
    public float ComebackStopDistance => _comebackStopDistance;

    public MonsterHealth Health => _health;
    public MonsterMovement Movement => _movement;
    public MonsterCombat Combat => _combat;
    public Transform PlayerTransform => _playerTransform;
    public Vector3 OriginPosition => _originPosition;

    private void Awake()
    {
        _health = GetComponent<MonsterHealth>();
        _movement = GetComponent<MonsterMovement>();
        _combat = GetComponent<MonsterCombat>();
    }

    private void Start()
    {
        InitializeReferences();
        ChangeState(new MonsterIdleState(this));
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

        _originPosition = transform.position;
    }

    public void ChangeState(IMonsterState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    public void ChangeState(EMonsterState stateType)
    {
        CurrentStateType = stateType;

        IMonsterState newState = stateType switch
        {
            EMonsterState.Idle => new MonsterIdleState(this),
            EMonsterState.Patrol => new MonsterPatrolState(this),
            EMonsterState.Trace => new MonsterTraceState(this),
            EMonsterState.Comeback => new MonsterComebackState(this),
            EMonsterState.Attack => new MonsterAttackState(this),
            EMonsterState.Hit => new MonsterHitState(this),
            EMonsterState.Death => new MonsterDeadState(this),
            _ => new MonsterIdleState(this)
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
}