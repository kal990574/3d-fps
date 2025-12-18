using UnityEngine;
using UnityEngine.AI;

public class PlayerRotate : MonoBehaviour
{
    private const float RotationSpeed = 10f;

    [Header("References")]
    [SerializeField] private CameraModeSwitch _cameraModeSwitch;

    private Camera _mainCamera;
    private NavMeshAgent _agent;

    private void Start()
    {
        _mainCamera = Camera.main;
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (_cameraModeSwitch != null && _cameraModeSwitch.IsQuarterView)
        {
            RotateTowardsMovementDirection();
            return;
        }

        RotateTowardsCamera();
    }

    private void RotateTowardsCamera()
    {
        if (_mainCamera != null)
        {
            float cameraYRotation = _mainCamera.transform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0, cameraYRotation, 0);
        }
    }

    private void RotateTowardsMovementDirection()
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
}