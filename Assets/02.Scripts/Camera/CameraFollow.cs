using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _noseTransform;
    [SerializeField] private CameraModeSwitch _modeSwitch;
    [SerializeField] private CameraRotate _cameraRotate;

    [Header("Smooth Follow")]
    [SerializeField] private float _smoothTime = 0.1f;
    [SerializeField] private float _velocityThreshold = 0.01f;

    [Header("Camera Collision")]
    [SerializeField] private LayerMask _cameraCollisionLayers;
    [SerializeField] private float _cameraRadius = 0.2f;

    private Vector3 _velocity = Vector3.zero;
    private bool _wasColliding = false;

    private void Start()
    {
        if (_modeSwitch == null)
        {
            _modeSwitch = GetComponent<CameraModeSwitch>();
        }

        if (_cameraRotate == null)
        {
            _cameraRotate = GetComponent<CameraRotate>();
        }
    }

    void LateUpdate()
    {
        if (_noseTransform == null || _modeSwitch == null)
        {
            return;
        }

        Vector3 desiredPosition = CalculateDesiredPosition();

        Vector3 direction = desiredPosition - _noseTransform.position;
        float desiredDistance = direction.magnitude;

        if (desiredDistance > 0.1f && Physics.SphereCast(_noseTransform.position, _cameraRadius, direction.normalized, out RaycastHit hit, desiredDistance, _cameraCollisionLayers, QueryTriggerInteraction.Ignore))
        {
            //Debug.Log($"Camera Collision! Hit: {hit.collider.name}, Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}, Distance: {hit.distance:F2}");

            if (_cameraRotate != null)
            {
                _cameraRotate.SetWallCollision(true);
            }

            Vector3 collisionPosition = hit.point - direction.normalized * _cameraRadius;
            transform.position = Vector3.SmoothDamp(transform.position, collisionPosition, ref _velocity, _smoothTime);
            Debug.DrawLine(_noseTransform.position, hit.point, Color.red, 0.1f);

            _wasColliding = true;
        }
        else
        {
            if (_cameraRotate != null)
            {
                _cameraRotate.SetWallCollision(false);
            }

            if (_wasColliding && _velocity.magnitude > _velocityThreshold)
            {
                transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocity, _smoothTime);
                Debug.DrawLine(_noseTransform.position, desiredPosition, Color.yellow, 0.1f);
            }
            else
            {
                transform.position = desiredPosition;
                _velocity = Vector3.zero;
                _wasColliding = false;
                Debug.DrawLine(_noseTransform.position, desiredPosition, Color.green, 0.1f);
            }
        }
    }

    private Vector3 CalculateDesiredPosition()
    {
        if (_modeSwitch.IsQuarterView)
        {
            // 쿼터뷰: 월드 좌표 기준 오프셋
            return _noseTransform.position + _modeSwitch.CurrentOffset;
        }

        // FPS/TPS: 카메라 회전 기준 오프셋
        return _noseTransform.position + transform.TransformDirection(_modeSwitch.CurrentOffset);
    }
}
