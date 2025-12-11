using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour, IDamageable
{
    #region Comment
// 목표: 처음에는 가만히 있지만 플레이어가 다가가면 쫓아오는 좀비 몬스터를 만들고 싶다.
    //       ㄴ 쫓아 오다가 너무 멀어지면 제자리로 돌아간다.
    
    // Idle   : 가만히 있는다.
    //   I  (플레이어가 가까이 오면) (컨디션, 트랜지션)
    // Trace  : 플레이러를 쫒아간다.
    //   I  (플레이어와 너무 멀어지면)
    // Return : 제자리로 돌아가는 상태
    //   I  (제자리에 도착했다면)
    //  Idle
    // 공격
    // 피격
    // 죽음
    
    
    
    // 몬스터 인공지능(AI) : 사람처럼 행동하는 똑똑한 시스템/알고리즘
    // - 규칙 기반 인공지능 : 정해진 규칙에 따라 조건문/반복문등을 이용해서 코딩하는 것
    //                     -> ex) FSM(유한 상태머신), BT(행동 트리)
    // - 학습 기반 인공지능: 머신러닝(딥러닝, 강화학습 .. )
    
    // Finite State Machine(유한 상태 머신)
    // N개의 상태를 가지고 있고, 상태마다 행동이 다르다.
    

    #endregion

    public EMonsterState State = EMonsterState.Idle;
    [SerializeField] private GameObject _player;
    [SerializeField] private CharacterController _controller;

    [Header("Detection")]
    // 플레이어 탐지 범위
    public float DetectDistance = 4f;
    // 공격 가능 범위
    public float AttackDistance = 1.2f;
    // 최대 추적 거리 -> comeback
    public float MaxTraceDistance = 15f;
    [Header("Movement")]
    public float MoveSpeed = 5f;
    [Header("Attack")]
    public float AttackSpeed = 2f;
    public float AttackDamage = 10f;
    // 공격 타이머
    public float AttackTimer = 0f;

    [Header("Patrol")]
    // 순찰 반경
    public float PatrolRadius = 5f;
    // Idle -> Patrol 대기 시간
    public float PatrolIdleTime = 2f;

    [Header("Comeback")]
    // 복귀 완료 판정 거리
    public float ComebackStopDistance = 0.5f;

    [Header("Knockback")]
    public float KnockbackDistance = 2f;
    public float KnockbackDuration = 0.3f;


    private Vector3 _originPosition;
    private Vector3 _patrolTarget;
    // Idle 상태 경과 시간 타이머
    private float _patrolTimer = 0f;

    private void Start()
    {
        _originPosition = transform.position;
        _patrolTarget = _originPosition;

        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }

        if (_controller == null)
        {
            _controller = GetComponent<CharacterController>();
        }
    }

    private void Update()
    {
        // 몬스터의 상태에 따라 다른 행동을한다. (다른 메서드를 호출한다.)
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
    
    // 1. 함수는 한 가지 일만 잘해야 한다.
    // 2. 상태별 행동을 함수로 만든다.
    private void Idle()
    {
        // 대기하는 상태
        // Todo. Idle 애니메이션 실행

        // 플레이어 탐지
        if (Vector3.Distance(transform.position, _player.transform.position) <= DetectDistance)
        {
            State = EMonsterState.Trace;
            Debug.Log("상태 전환: Idle -> Trace");
            return;
        }

        // 일정 시간 후 순찰 시작
        _patrolTimer += Time.deltaTime;
        if (_patrolTimer >= PatrolIdleTime)
        {
            _patrolTimer = 0f;
            State = EMonsterState.Patrol;
            SetRandomPatrolTarget();
            Debug.Log("상태 전환: Idle -> Patrol");
        }
    }

    private void Patrol()
    {
        // 순찰하는 상태
        // Todo. Walk 애니메이션 실행

        // 플레이어 탐지
        if (Vector3.Distance(transform.position, _player.transform.position) <= DetectDistance)
        {
            State = EMonsterState.Trace;
            Debug.Log("상태 전환: Patrol -> Trace");
            return;
        }

        // 순찰 지점으로 이동
        Vector3 direction = (_patrolTarget - transform.position).normalized;
        direction.y = 0;

        _controller.Move(direction * MoveSpeed * Time.deltaTime);
        LookAtTarget(_patrolTarget);

        // 순찰 지점 도착
        float distanceXZ = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(_patrolTarget.x, 0, _patrolTarget.z));
        if (distanceXZ < 1f)
        {
            State = EMonsterState.Idle;
            _patrolTimer = 0f;
            Debug.Log($"상태 전환: Patrol -> Idle (순찰 지점 도착, 거리: {distanceXZ:F2}m)");
        }
    }

    private void Trace()
    {
        // 플레이어를 쫓아가는 상태
        // Todo. Run 애니메이션 실행

        float distance = Vector3.Distance(transform.position, _player.transform.position);

        // 최대 추적 거리 초과 → 복귀
        if (distance > MaxTraceDistance)
        {
            State = EMonsterState.Comeback;
            Debug.Log("상태 전환: Trace -> Comeback (추적 거리 초과)");
            return;
        }

        // 공격 범위 진입
        if (distance <= AttackDistance)
        {
            State = EMonsterState.Attack;
            Debug.Log("상태 전환: Trace -> Attack");
            return;
        }

        // 플레이어 추적
        Vector3 direction = (_player.transform.position - transform.position).normalized;
        _controller.Move(direction * MoveSpeed * Time.deltaTime);
        LookAtTarget(_player.transform.position);
    }

    private void Comeback()
    {
        // 제자리로 복귀하는 상태
        // Todo. Walk 애니메이션 실행

        // 초기 위치로 이동
        Vector3 direction = (_originPosition - transform.position).normalized;
        direction.y = 0;

        _controller.Move(direction * MoveSpeed * Time.deltaTime);
        LookAtTarget(_originPosition);

        // 복귀 완료
        if (Vector3.Distance(transform.position, _originPosition) < ComebackStopDistance)
        {
            State = EMonsterState.Idle;
            _patrolTimer = 0f;
            Debug.Log("상태 전환: Comeback -> Idle (복귀 완료)");
        }
    }

    private void Attack()
    {
        // 플레이어를 공격하는 상태
        // Todo. Attack 애니메이션 실행

        // 플레이어와의 거리가 멀다면 다시 쫒아오는 상태로 전환
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance > AttackDistance)
        {
            State = EMonsterState.Trace;
            Debug.Log("상태 전환: Attack -> Trace");
            return;
        }

        // 플레이어 바라보기
        LookAtTarget(_player.transform.position);

        // 공격 쿨타임
        AttackTimer += Time.deltaTime;
        if (AttackTimer >= AttackSpeed)
        {
            AttackTimer = 0f;

            // 플레이어 데미지 처리
            IDamageable playerStats = _player.GetComponent<IDamageable>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(AttackDamage);
                Debug.Log($"플레이어 공격! 데미지: {AttackDamage}");
            }
        }
    }


    public float Health = 100;

    public void TakeDamage(float damage)
    {
        if (State == EMonsterState.Hit || State == EMonsterState.Death)
        {
            return;
        }

        Health -= damage;

        if (Health > 0)
        {
            Debug.Log($"상태 전환: {State} -> Hit (데미지: {damage})");
            State = EMonsterState.Hit;
            StartCoroutine(Hit_Coroutine());
        }
        else
        {
            Debug.Log($"상태 전환: {State} -> Death (데미지: {damage})");
            State = EMonsterState.Death;
            StartCoroutine(Death_Coroutine());
        }
    }
    
    private IEnumerator Hit_Coroutine()
    {
        // Todo. Hit 애니메이션 실행

        // 피격 방향 계산 
        Vector3 knockbackDirection = (transform.position - _player.transform.position).normalized;
        knockbackDirection.y = 0;

        // 넉백 이동
        float elapsed = 0f;
        while (elapsed < KnockbackDuration)
        {
            float progress = elapsed / KnockbackDuration;
            float strength = Mathf.Lerp(1f, 0f, progress);

            _controller.Move(knockbackDirection * KnockbackDistance * strength * Time.deltaTime / KnockbackDuration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Idle로 복귀
        State = EMonsterState.Idle;
    }

    private IEnumerator Death_Coroutine()
    {
        // Todo. Death 애니메이션 실행
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    private void LookAtTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void SetRandomPatrolTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * PatrolRadius;
        _patrolTarget = _originPosition + new Vector3(randomCircle.x, 0, randomCircle.y);
        Debug.Log($"새 순찰 지점 설정: {_patrolTarget} (초기 위치: {_originPosition}, 반경: {PatrolRadius}m)");
    }
}