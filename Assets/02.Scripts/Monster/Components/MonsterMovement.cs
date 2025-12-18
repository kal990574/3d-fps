using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MonsterMovement : MonoBehaviour
{
    private const float RotationSpeed = 5f;
    private const int MaxRandomPointAttempts = 30;

    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _angularSpeed = 120f;
    [SerializeField] private float _acceleration = 8f;
    [SerializeField] private float _stoppingDistance = 0.5f;

    [Header("Knockback Settings")]
    [SerializeField] private float _knockbackDuration = 0.3f;

    [Header("Jump Settings")]
    [SerializeField] private float _jumpHeight = 2f;
    [SerializeField] private float _jumpDuration = 0.5f;

    private NavMeshAgent _agent;
    private CharacterController _controller;
    private bool _isKnockbackActive;
    private bool _isJumping;
    private Vector3 _cachedLinkEndPosition;
    private bool _hasValidLinkData;

    public float MoveSpeed => _moveSpeed;
    public float CurrentSpeed => _agent != null ? _agent.velocity.magnitude : 0f;
    public bool IsKnockbackActive => _isKnockbackActive;
    public bool IsJumping => _isJumping;

    public bool IsOnOffMeshLink
    {
        get
        {
            if (_agent != null && _agent.isOnOffMeshLink)
            {
                CacheLinkData();
                return true;
            }

            return false;
        }
    }

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _controller = GetComponent<CharacterController>();
        ConfigureAgent();
    }

    private void ConfigureAgent()
    {
        if (_agent == null)
        {
            return;
        }

        _agent.speed = _moveSpeed;
        _agent.angularSpeed = _angularSpeed;
        _agent.acceleration = _acceleration;
        _agent.stoppingDistance = _stoppingDistance;
        _agent.updateRotation = false;
        _agent.autoTraverseOffMeshLink = false;
    }

    public void MoveTowards(Vector3 target, float speedMultiplier = 1f)
    {
        if (_isKnockbackActive || _agent == null || !_agent.enabled || !_agent.isOnNavMesh)
        {
            return;
        }

        _agent.speed = _moveSpeed * speedMultiplier;
        _agent.SetDestination(target);
    }

    public void LookAtTarget(Vector3 targetPosition)
    {
        Vector3 direction = CalculateDirectionToTarget(targetPosition);

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        }
    }

    public float GetDistanceTo(Vector3 target)
    {
        return Vector3.Distance(transform.position, target);
    }

    public float GetHorizontalDistanceTo(Vector3 target)
    {
        float dx = target.x - transform.position.x;
        float dz = target.z - transform.position.z;
        return Mathf.Sqrt(dx * dx + dz * dz);
    }

    public bool TryGetRandomNavMeshPoint(Vector3 center, float radius, out Vector3 result)
    {
        for (int i = 0; i < MaxRandomPointAttempts; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * radius;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, radius, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = center;
        return false;
    }

    public void StopMovement()
    {
        if (_agent != null && _agent.enabled && _agent.isOnNavMesh)
        {
            _agent.ResetPath();
            _agent.velocity = Vector3.zero;
        }
    }

    public void ApplyKnockback(Vector3 direction, float distance)
    {
        if (_isKnockbackActive)
        {
            return;
        }

        StartCoroutine(ApplyKnockbackCoroutine(direction, distance, _knockbackDuration));
    }

    private IEnumerator ApplyKnockbackCoroutine(Vector3 direction, float distance, float duration)
    {
        _isKnockbackActive = true;

        if (_agent != null && _agent.enabled)
        {
            _agent.ResetPath();
            _agent.enabled = false;
        }

        float knockbackSpeed = distance / duration;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float progress = elapsed / duration;
            float strength = Mathf.Lerp(1f, 0f, progress);
            Vector3 movement = direction * (knockbackSpeed * strength * Time.deltaTime);

            if (_controller != null && _controller.enabled)
            {
                _controller.Move(movement);
            }
            else
            {
                transform.position += movement;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return StartCoroutine(WaitForGroundedCoroutine());

        WarpToNearestNavMeshPosition();

        if (_agent != null)
        {
            _agent.enabled = true;
        }

        _isKnockbackActive = false;
    }

    private IEnumerator WaitForGroundedCoroutine()
    {
        float gravity = 9.81f;
        float verticalVelocity = 0f;
        float maxWaitTime = 2f;
        float elapsed = 0f;

        while (elapsed < maxWaitTime)
        {
            if (_controller != null && _controller.enabled)
            {
                if (_controller.isGrounded)
                {
                    yield break;
                }

                verticalVelocity -= gravity * Time.deltaTime;
                _controller.Move(new Vector3(0, verticalVelocity * Time.deltaTime, 0));
            }
            else
            {
                if (Physics.Raycast(transform.position, Vector3.down, 0.1f))
                {
                    yield break;
                }

                verticalVelocity -= gravity * Time.deltaTime;
                transform.position += new Vector3(0, verticalVelocity * Time.deltaTime, 0);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void WarpToNearestNavMeshPosition()
    {
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
    }

    private Vector3 CalculateDirectionToTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0;
        return direction;
    }

    public bool TryGetCurrentLinkEndPosition(out Vector3 endPosition)
    {
        if (_hasValidLinkData)
        {
            endPosition = _cachedLinkEndPosition;
            return true;
        }

        endPosition = Vector3.zero;
        return false;
    }

    private void CacheLinkData()
    {
        if (_agent != null && _agent.isOnOffMeshLink)
        {
            OffMeshLinkData linkData = _agent.currentOffMeshLinkData;
            _cachedLinkEndPosition = linkData.endPos;
            _hasValidLinkData = true;
        }
    }

    private void ClearLinkCache()
    {
        _hasValidLinkData = false;
        _cachedLinkEndPosition = Vector3.zero;
    }

    public void StartJump(Vector3 endPosition)
    {
        if (_isJumping)
        {
            return;
        }

        StartCoroutine(JumpCoroutine(endPosition));
    }

    public void CompleteOffMeshLink()
    {
        if (_agent != null && _agent.isOnOffMeshLink)
        {
            _agent.CompleteOffMeshLink();
        }

        ClearLinkCache();
        _agent.isStopped = false;
    }

    private IEnumerator JumpCoroutine(Vector3 endPosition)
    {
        _isJumping = true;
        _agent.isStopped = true;
        _agent.updatePosition = false;

        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < _jumpDuration)
        {
            float t = elapsed / _jumpDuration;

            Vector3 horizontalPos = Vector3.Lerp(startPosition, endPosition, t);
            float height = Mathf.Sin(t * Mathf.PI) * 
                           _jumpHeight;

            transform.position = new Vector3(
                horizontalPos.x,
                Mathf.Lerp(startPosition.y, endPosition.y, t) + height,
                horizontalPos.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
        _agent.Warp(endPosition);
        _agent.updatePosition = true;
        _isJumping = false;
    }
}