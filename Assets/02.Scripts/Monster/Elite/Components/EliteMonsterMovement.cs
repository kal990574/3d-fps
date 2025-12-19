using System.Collections;
using UnityEngine;

public class EliteMonsterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _rotationSpeed = 5f;

    [Header("Knockback Settings")]
    [SerializeField] private float _knockbackDuration = 0.3f;

    private CharacterController _controller;
    private bool _isKnockbackActive;
    private float _verticalVelocity;
    private const float Gravity = -20f;

    public float CurrentSpeed { get; private set; }
    public bool IsKnockbackActive => _isKnockbackActive;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    public void MoveTowards(Vector3 targetPosition)
    {
        if (_isKnockbackActive || _controller == null)
        {
            return;
        }

        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        ApplyGravity();

        Vector3 move = direction * _moveSpeed + Vector3.up * _verticalVelocity;
        _controller.Move(move * Time.deltaTime);

        CurrentSpeed = new Vector3(direction.x, 0, direction.z).magnitude * _moveSpeed;
    }

    public void LookAtTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                _rotationSpeed * Time.deltaTime
            );
        }
    }

    public float GetDistanceTo(Vector3 target)
    {
        return Vector3.Distance(transform.position, target);
    }

    public void StopMovement()
    {
        CurrentSpeed = 0f;
    }

    public void ApplyKnockback(Vector3 direction, float distance)
    {
        if (_isKnockbackActive)
        {
            return;
        }

        StartCoroutine(KnockbackCoroutine(direction, distance));
    }

    private void ApplyGravity()
    {
        if (_controller.isGrounded)
        {
            _verticalVelocity = -2f;
        }
        else
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    private IEnumerator KnockbackCoroutine(Vector3 direction, float distance)
    {
        _isKnockbackActive = true;

        float knockbackSpeed = distance / _knockbackDuration;
        float elapsed = 0f;

        while (elapsed < _knockbackDuration)
        {
            float progress = elapsed / _knockbackDuration;
            float strength = Mathf.Lerp(1f, 0f, progress);
            Vector3 movement = direction * (knockbackSpeed * strength * Time.deltaTime);

            _controller.Move(movement);

            elapsed += Time.deltaTime;
            yield return null;
        }

        _isKnockbackActive = false;
    }
}