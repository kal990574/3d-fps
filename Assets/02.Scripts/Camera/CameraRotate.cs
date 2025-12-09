using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    [Header("Mouse Settings")]
    public float MouseSensitivity = 2f;
    public float MaxVerticalAngle = 90f;
    public float MinVerticalAngle = -90f;

    private float _rotationX = 0f; // 상하 회전 
    private float _rotationY = 0f; // 좌우 회전 

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 회전 누적 
        _rotationX -= mouseY * MouseSensitivity;
        _rotationY += mouseX * MouseSensitivity;

        // 상하 각도 제한 
        _rotationX = Mathf.Clamp(_rotationX, MinVerticalAngle, MaxVerticalAngle);

        // Quaternion으로 회전 적용 
        transform.localRotation = Quaternion.Euler(_rotationX, _rotationY, 0f);
    }
}
