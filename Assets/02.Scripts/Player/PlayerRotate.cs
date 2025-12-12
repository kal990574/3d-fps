using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (_mainCamera != null)
        {
            float cameraYRotation = _mainCamera.transform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0, cameraYRotation, 0);
        }
    }
}
