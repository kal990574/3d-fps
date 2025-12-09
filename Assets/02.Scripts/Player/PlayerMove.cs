using UnityEngine;

// 키보드를 누르면 캐릭터를 그 방향으로 이동 시키고 싶다.
public class PlayerMove : MonoBehaviour
{
    // 이동 관련
    public float MoveSpeed = 7;
    public float SprintSpeed = 12;
    private CharacterController _characterController;
    public float Gravity = 9.81f;
    private float _yVelocity;

    // 스태미나 관련
    [Header("Stamina")]
    public float MaxStamina = 100f;
    public float StaminaDrainRate = 20f;    // 초당 소모량
    public float StaminaRecoveryRate = 10f; // 초당 회복량

    private float _currentStamina;
    public float CurrentStamina => _currentStamina; // UI에서 접근용

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _currentStamina = MaxStamina;
    }

    private void Update()
    {
        _yVelocity += Gravity * Time.deltaTime;

        // 1. 키보드 입력 받기
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        bool isMoving = x != 0 || y != 0;

        // 2. 달리기 입력 확인
        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift);
        bool isSprinting = wantsToSprint && isMoving && _currentStamina > 0;

        // 3. 스태미나 처리
        if (isSprinting)
        {
            // 달리는 중 - 스태미나 소모
            _currentStamina -= StaminaDrainRate * Time.deltaTime;
            _currentStamina = Mathf.Max(_currentStamina, 0);
        }
        else
        {
            // 달리지 않음 - 스태미나 회복
            _currentStamina += StaminaRecoveryRate * Time.deltaTime;
            _currentStamina = Mathf.Min(_currentStamina, MaxStamina);
        }

        // 4. 이동 속도 결정
        float currentSpeed = isSprinting ? SprintSpeed : MoveSpeed;

        // 5. 입력에 따른 방향 구하기
        Vector3 direction = new Vector3(x, 0, y);
        direction.Normalize();
        // 카메라가 쳐다보는 방향으로 변환
        direction = Camera.main.transform.TransformDirection(direction);
        direction.y -= Gravity;

        // 6. 이동
        _characterController.Move(direction * currentSpeed * Time.deltaTime);
    }
}