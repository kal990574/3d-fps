using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _offsetY = 50f;

    private void LateUpdate()
    {
        if (_target == null)
        {
            return;
        }

        Vector3 targetPosition = _target.position;
        Vector3 finalPosition = targetPosition + new Vector3(0f, _offsetY, 0f);
        transform.position = finalPosition;

        Vector3 targetAngle = _target.eulerAngles;
        targetAngle.x = 90f;
        transform.eulerAngles = targetAngle;
    }
}