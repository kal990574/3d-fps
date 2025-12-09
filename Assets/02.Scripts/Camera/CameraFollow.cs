using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _noseTransform;

    [Header("Camera Mode")]
    [SerializeField] private bool _isFPSMode = true;
    [SerializeField] private Vector3 _fpsOffset = Vector3.zero;           // FPS: 코 위치
    [SerializeField] private Vector3 _tpsOffset = new Vector3(0, 2, -5);  // TPS: 뒤쪽 위치
    [SerializeField] private float _transitionDuration = 0.5f;            // 전환 시간

    private Vector3 _currentLocalOffset;
    private Tween _currentTween;

    private void Start()
    {
        // 초기 오프셋 설정
        _currentLocalOffset = _isFPSMode ? _fpsOffset : _tpsOffset;
    }

    void Update()
    {
        // T키로 모드 전환
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleCameraMode();
        }
    }

    void LateUpdate()
    {
        // 카메라가 nose 포지션 + 오프셋을 따라가기
        if (_noseTransform != null)
        {
            // 카메라 자신의 회전 기준으로 오프셋 적용
            Vector3 targetPosition = _noseTransform.position + transform.TransformDirection(_currentLocalOffset);
            transform.position = targetPosition;
        }
    }

    private void ToggleCameraMode()
    {
        _isFPSMode = !_isFPSMode;

        // 진행 중인 Tween 취소
        _currentTween?.Kill();

        // 목표 오프셋
        Vector3 targetOffset = _isFPSMode ? _fpsOffset : _tpsOffset;

        // DOTween으로 부드러운 전환
        _currentTween = DOTween.To(
            () => _currentLocalOffset,
            x => _currentLocalOffset = x,
            targetOffset,
            _transitionDuration
        ).SetEase(Ease.OutCubic);
    }
}
