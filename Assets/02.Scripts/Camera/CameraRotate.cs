using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    [Header("Mouse Settings")]
    public float MouseSensitivity = 2f;
    public float MaxVerticalAngle = 90f;
    public float MinVerticalAngle = -90f;

    private float _rotationX;
    private float _rotationY;

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

        _rotationX -= mouseY * MouseSensitivity;
        _rotationY += mouseX * MouseSensitivity;

        _rotationX = Mathf.Clamp(_rotationX, MinVerticalAngle, MaxVerticalAngle);
    }

    private void ApplyRotation()
    {
        transform.localRotation = Quaternion.Euler(_rotationX, _rotationY, 0f);
    }

    public void ApplyRecoil(float verticalKick, float horizontalKick)
    {
        _rotationX -= verticalKick;
        _rotationY += horizontalKick;

        _rotationX = Mathf.Clamp(_rotationX, MinVerticalAngle, MaxVerticalAngle);
    }
}
