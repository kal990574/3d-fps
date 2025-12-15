using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    [Header("Mouse Settings")]
    public float MouseSensitivity = 2f;
    public float MaxVerticalAngle = 90f;
    public float MinVerticalAngle = -90f;

    [Header("Wall Collision Limit")]
    [SerializeField] private float _minPitchNearWall = -30f;
    [SerializeField] private float _maxPitchNearWall = 30f;
    [SerializeField] private float _pitchLerpSpeed = 10f;

    private float _pitch;
    private float _yaw;
    private float _currentMinPitch;
    private float _currentMaxPitch;

    public float Pitch => _pitch;

    private void Start()
    {
        _currentMinPitch = MinVerticalAngle;
        _currentMaxPitch = MaxVerticalAngle;
    }

    private void Update()
    {
        HandleMouseInput();
        ApplyRotation();
    }

    private void HandleMouseInput()
    {
        float mouseX = InputManager.Instance.MouseX;
        float mouseY = InputManager.Instance.MouseY;

        _pitch -= mouseY * MouseSensitivity;
        _yaw += mouseX * MouseSensitivity;

        _pitch = Mathf.Clamp(_pitch, _currentMinPitch, _currentMaxPitch);
    }

    private void ApplyRotation()
    {
        transform.localRotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }

    public void ApplyRecoil(float verticalKick, float horizontalKick)
    {
        _pitch -= verticalKick;
        _yaw += horizontalKick;

        _pitch = Mathf.Clamp(_pitch, _currentMinPitch, _currentMaxPitch);
    }

    public void SetWallCollision(bool isNearWall)
    {
        float targetMinPitch = isNearWall ? _minPitchNearWall : MinVerticalAngle;
        float targetMaxPitch = isNearWall ? _maxPitchNearWall : MaxVerticalAngle;

        _currentMinPitch = Mathf.Lerp(_currentMinPitch, targetMinPitch, Time.deltaTime * _pitchLerpSpeed);
        _currentMaxPitch = Mathf.Lerp(_currentMaxPitch, targetMaxPitch, Time.deltaTime * _pitchLerpSpeed);

        if (_pitch < _currentMinPitch)
        {
            _pitch = _currentMinPitch;
        }

        if (_pitch > _currentMaxPitch)
        {
            _pitch = _currentMaxPitch;
        }
    }
}
