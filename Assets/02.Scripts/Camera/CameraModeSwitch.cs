using System.Numerics;
using UnityEngine;
using DG.Tweening;
using Vector3 = UnityEngine.Vector3;

public class CameraModeSwitch : MonoBehaviour
{
    [Header("Camera Mode")]
    [SerializeField] private CameraMode _currentMode = CameraMode.FPS;
    [SerializeField] private Vector3 _fpsOffset = Vector3.zero;
    [SerializeField] private Vector3 _tpsOffset = new Vector3(0, 2, -5);
    [SerializeField] private Vector3 _quarterOffset = new Vector3(0, 20, -20);

    [Header("Quarter View Settings")]
    [SerializeField] private Vector3 _quarterRotation = new Vector3(45, 0, 0);

    [Header("Transition")]
    [SerializeField] private float _transitionDuration = 0.5f;

    private Vector3 _currentLocalOffset;
    private Tween _currentTween;

    public Vector3 CurrentOffset => _currentLocalOffset;
    public bool IsQuarterView => _currentMode == CameraMode.QuarterView;
    public Vector3 QuarterRotation => _quarterRotation;

    private void Start()
    {
        _currentLocalOffset = GetOffsetForMode(_currentMode);
    }

    private void Update()
    {
        if (InputManager.Instance.CameraTogglePressed)
        {
            CycleNextMode();
        }
    }

    private void CycleNextMode()
    {
        _currentMode = _currentMode switch
        {
            CameraMode.FPS => CameraMode.TPS,
            CameraMode.TPS => CameraMode.QuarterView,
            CameraMode.QuarterView => CameraMode.FPS,
            _ => CameraMode.FPS
        };

        TransitionToCurrentMode();
    }

    private void TransitionToCurrentMode()
    {
        _currentTween?.Kill();

        Vector3 targetOffset = GetOffsetForMode(_currentMode);

        _currentTween = DOTween.To(
            () => _currentLocalOffset,
            x => _currentLocalOffset = x,
            targetOffset,
            _transitionDuration
        ).SetEase(Ease.OutCubic);
    }

    private Vector3 GetOffsetForMode(CameraMode mode)
    {
        return mode switch
        {
            CameraMode.FPS => _fpsOffset,
            CameraMode.TPS => _tpsOffset,
            CameraMode.QuarterView => _quarterOffset,
            _ => _fpsOffset
        };
    }
}