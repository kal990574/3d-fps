using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    // Movement
    public float Horizontal => Input.GetAxis("Horizontal");
    public float Vertical => Input.GetAxis("Vertical");
    public bool JumpPressed => Input.GetKeyDown(KeyCode.Space);
    public bool SprintHeld => Input.GetKey(KeyCode.LeftShift);

    // Combat
    public bool FireHeld => Input.GetMouseButton(0);
    public bool FirePressed => Input.GetMouseButtonDown(0);
    public bool RightFirePressed => Input.GetMouseButtonDown(1);
    public bool ReloadPressed => Input.GetKeyDown(KeyCode.R);

    // Camera
    public float MouseX => Input.GetAxis("Mouse X");
    public float MouseY => Input.GetAxis("Mouse Y");
    public bool CameraTogglePressed => Input.GetKeyDown(KeyCode.V);

    // System
    public bool EscapePressed => Input.GetKeyDown(KeyCode.Escape);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
