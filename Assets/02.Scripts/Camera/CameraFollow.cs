using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _noseTransform;
    [SerializeField] private CameraModeSwitch _modeSwitch;

    private void Start()
    {
        // CameraModeSwitch를 자동으로 찾기 (할당 안 했을 경우)
        if (_modeSwitch == null)
        {
            _modeSwitch = GetComponent<CameraModeSwitch>();
        }
    }

    void LateUpdate()
    {
        // 카메라가 nose 포지션 + 오프셋을 따라가기
        if (_noseTransform != null && _modeSwitch != null)
        {
            // 카메라 자신의 회전 기준으로 오프셋 적용
            Vector3 targetPosition = _noseTransform.position + transform.TransformDirection(_modeSwitch.CurrentOffset);
            transform.position = targetPosition;
        }
    }
}
