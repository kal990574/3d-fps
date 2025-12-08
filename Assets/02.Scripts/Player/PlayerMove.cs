using UnityEngine;

// 키보드를 누르면 캐릭터를 그 방향으로 이동 시키고 싶다.
public class PlayerMove : MonoBehaviour
{
    // 필요 속성
    // - 이동속도
    public float MoveSpeed = 7;
    private CharacterController _characterController;
    public float Gravity = 9.81f;
    private float _yVelocity;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        _yVelocity += Gravity * Time.deltaTime;
        // 1. 키보드 입력 받기
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        
        // 2. 입력에 따른 방향 구하기 
        // 현재는 유니티 세상의 절대적인 방향이 기준 (글로벌/월드 좌표계)
        // 내가 원하는 것은 카메라가 쳐다보는 방향이 기준으로
        
        // - 글로벌 좌표 방향을 구한다. 
        Vector3 direction = new Vector3(x, 0, y);
        direction.Normalize();
        // - 카메라가 쳐다보는 방향으로 변환한다. (월드 -> 로컬)
        direction = Camera.main.transform.TransformDirection(direction);
        direction.y -= Gravity;
        
        
        _characterController.Move(direction * MoveSpeed * Time.deltaTime);
    }
    
}