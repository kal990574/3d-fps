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

    private NavMeshAgent _agent;
    private CharacterController _controller;
    private bool _isKnockbackActive;

    public float MoveSpeed => _moveSpeed;
    public bool IsKnockbackActive => _isKnockbackActive;

    public bool HasReachedDestination
    {
        get
        {
            if (_agent == null || !_agent.enabled || !_agent.isOnNavMesh)
            {
                return true;
            }

            return !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance;
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

    public bool HasValidPathTo(Vector3 target)
    {
        if (_agent == null || !_agent.enabled || !_agent.isOnNavMesh)
        {
            return false;
        }

        NavMeshPath path = new NavMeshPath();
        _agent.CalculatePath(target, path);
        return path.status == NavMeshPathStatus.PathComplete;
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
}