using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    private void Update()
    {
        // 플레이어를 카메라의 Y축 회전에 맞춤
        if (Camera.main != null)
        {
            float cameraYRotation = Camera.main.transform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0, cameraYRotation, 0);
        }
    }
}
