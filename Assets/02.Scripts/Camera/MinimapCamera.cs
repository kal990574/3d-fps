using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _offsetY = 50f;

    [Header("Zoom Settings")]
    [SerializeField] private float _minZoom = 10f;
    [SerializeField] private float _maxZoom = 50f;
    [SerializeField] private float _zoomStep = 10f;

    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

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

    public void ZoomIn()
    {
        if (_camera == null)
        {
            return;
        }

        _camera.orthographicSize = Mathf.Max(_minZoom, _camera.orthographicSize - _zoomStep);
    }

    public void ZoomOut()
    {
        if (_camera == null)
        {
            return;
        }

        _camera.orthographicSize = Mathf.Min(_maxZoom, _camera.orthographicSize + _zoomStep);
    }
}