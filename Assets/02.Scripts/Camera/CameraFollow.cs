using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _noseTransform;
    [SerializeField] private CameraModeSwitch _modeSwitch;

    private void Start()
    {
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
