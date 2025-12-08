using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _noseTransform;

    void LateUpdate()
    {
        // 카메라가 nose 포지션을 따라가기
        if (_noseTransform != null)
        {
            transform.position = _noseTransform.position;
        }
    }
}
