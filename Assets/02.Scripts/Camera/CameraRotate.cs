using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public float RotationSpeed = 150f;
    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        Vector3 direction = new Vector3(-mouseY, mouseX, 0).normalized;
        transform.eulerAngles += RotationSpeed * direction * Time.deltaTime;
    }
}
