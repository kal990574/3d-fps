using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    private const float RotationSpeed = 5f;

    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;

    private CharacterController _controller;

    public float MoveSpeed => _moveSpeed;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    public void MoveTowards(Vector3 target, float speedMultiplier = 1f)
    {
        Vector3 direction = CalculateDirectionToTarget(target);
        _controller.Move(direction * (_moveSpeed * speedMultiplier * Time.deltaTime));
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

    private Vector3 CalculateDirectionToTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0;
        return direction;
    }
}