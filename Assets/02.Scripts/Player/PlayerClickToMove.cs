using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerClickToMove : MonoBehaviour
{
    private const float RotationSpeed = 10f;

    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 7f;
    [SerializeField] private float _angularSpeed = 720f;
    [SerializeField] private float _acceleration = 8f;
    [SerializeField] private float _stoppingDistance = 0.1f;

    [Header("Jump Settings")]
    [SerializeField] private float _jumpHeight = 2f;
    [SerializeField] private float _jumpDuration = 0.5f;

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask _groundLayerMask = ~0;
    [SerializeField] private float _maxRaycastDistance = 100f;

    private NavMeshAgent _agent;
    private CharacterController _controller;
    private Camera _mainCamera;
    private bool _isActive;
    private bool _isJumping;
    private Vector3 _cachedLinkEndPosition;
    private bool _hasValidLinkData;

    public bool IsActive => _isActive;
    public bool IsMoving => _agent != null && _agent.enabled && _agent.hasPath && !_agent.isStopped;
    public bool IsJumping => _isJumping;
    public NavMeshAgent Agent => _agent;

    public bool IsOnOffMeshLink
    {
        get
        {
            if (_agent != null && _agent.enabled && _agent.isOnOffMeshLink)
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
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        ConfigureAgent();

        if (_mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
        }
    }

    private void Update()
    {
        if (!_isActive)
        {
            return;
        }

        HandleOffMeshLink();
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

    public void Enable()
    {
        _isActive = true;

        if (_controller != null)
        {
            _controller.enabled = false;
        }

        if (_agent != null)
        {
            _agent.enabled = true;
            WarpToNearestNavMeshPosition();
        }
    }

    public void Disable()
    {
        _isActive = false;
        StopMovement();

        if (_agent != null)
        {
            _agent.enabled = false;
        }

        if (_controller != null)
        {
            _controller.enabled = true;
        }
    }

    public void TryMoveToMousePosition()
    {
        if (!_isActive || _agent == null || !_agent.enabled)
        {
            return;
        }

        if (TryGetNavMeshPosition(Input.mousePosition, out Vector3 targetPosition))
        {
            MoveToPosition(targetPosition);
        }
    }

    public void MoveToPosition(Vector3 targetPosition)
    {
        if (_agent == null || !_agent.enabled || !_agent.isOnNavMesh || _isJumping)
        {
            return;
        }

        _agent.SetDestination(targetPosition);
    }

    public void StopMovement()
    {
        if (_agent != null && _agent.enabled && _agent.isOnNavMesh)
        {
            _agent.ResetPath();
            _agent.velocity = Vector3.zero;
        }
    }

    public void LookAtMovementDirection()
    {
        if (_agent == null || !_agent.enabled || _agent.velocity.sqrMagnitude < 0.01f)
        {
            return;
        }

        Vector3 direction = _agent.velocity.normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        }
    }

    private bool TryGetNavMeshPosition(Vector3 screenPosition, out Vector3 navMeshPosition)
    {
        navMeshPosition = Vector3.zero;

        if (_mainCamera == null)
        {
            return false;
        }

        Ray ray = _mainCamera.ScreenPointToRay(screenPosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, _maxRaycastDistance, _groundLayerMask))
        {
            return false;
        }

        if (!NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
        {
            return false;
        }

        navMeshPosition = navHit.position;
        return true;
    }

    private void WarpToNearestNavMeshPosition()
    {
        if (_agent == null)
        {
            return;
        }

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            _agent.Warp(hit.position);
        }
        else
        {
            Debug.LogWarning("NavMesh position not found. Click to move may not work.");
        }
    }

    private void HandleOffMeshLink()
    {
        if (!IsOnOffMeshLink || _isJumping)
        {
            return;
        }

        if (TryGetCurrentLinkEndPosition(out Vector3 endPosition))
        {
            StartJump(endPosition);
        }
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

        if (_agent != null)
        {
            _agent.isStopped = false;
        }
    }

    private IEnumerator JumpCoroutine(Vector3 endPosition)
    {
        _isJumping = true;

        if (_agent != null)
        {
            _agent.isStopped = true;
            _agent.updatePosition = false;
        }

        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < _jumpDuration)
        {
            float t = elapsed / _jumpDuration;

            Vector3 horizontalPos = Vector3.Lerp(startPosition, endPosition, t);
            float height = Mathf.Sin(t * Mathf.PI) * _jumpHeight;

            transform.position = new Vector3(
                horizontalPos.x,
                Mathf.Lerp(startPosition.y, endPosition.y, t) + height,
                horizontalPos.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;

        if (_agent != null)
        {
            _agent.Warp(endPosition);
            _agent.updatePosition = true;
        }

        _isJumping = false;
        CompleteOffMeshLink();
    }
}