using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    [Header("Mouse Settings")]
    public float MouseSensitivity = 2f;
    public float MaxVerticalAngle = 90f;
    public float MinVerticalAngle = -90f;

    private float _pitch;
    private float _yaw;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleCursorLock();
        HandleMouseInput();
        ApplyRotation();
    }

    private void HandleCursorLock()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            return;
        }

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void HandleMouseInput()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        _pitch -= mouseY * MouseSensitivity;
        _yaw += mouseX * MouseSensitivity;

        _pitch = Mathf.Clamp(_pitch, MinVerticalAngle, MaxVerticalAngle);
    }

    private void ApplyRotation()
    {
        transform.localRotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }

    public void ApplyRecoil(float verticalKick, float horizontalKick)
    {
        _pitch -= verticalKick;
        _yaw += horizontalKick;

        _pitch = Mathf.Clamp(_pitch, MinVerticalAngle, MaxVerticalAngle);
    }
}
