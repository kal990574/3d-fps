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

    // 점프 관련
    [Header("Jump")]
    public float JumpPower = 10f;
    public float DoubleJumpStaminaCost = 25f; // 2단 점프 스태미나 소모량
    private int _jumpCount = 0;

    // 스태미나 관련
    [Header("Stamina")]
    public float MaxStamina = 100f;
    public float StaminaDrainRate = 20f;    // 초당 소모량
    public float StaminaRecoveryRate = 10f; // 초당 회복량

    private float _currentStamina;
    public float CurrentStamina => _currentStamina; // UI에서 접근용

    // 체력 관련
    [Header("Health")]
    public float MaxHealth = 100f;
    private float _currentHealth;
    public float CurrentHealth => _currentHealth; // UI에서 접근용

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _currentStamina = MaxStamina;
        _currentHealth = MaxHealth;
    }

    private void Update()
    {
        // 땅에 닿아있는지 체크
        bool isGrounded = _characterController.isGrounded;

        if (isGrounded)
        {
            _yVelocity = 0;
            _jumpCount = 0; // 땅에 닿으면 점프 카운트 리셋
        }
        else
        {
            _yVelocity -= Gravity * Time.deltaTime;
        }

        // 1. 키보드 입력 받기
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        bool isMoving = x != 0 || y != 0;

        // 2. 점프 처리
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_jumpCount == 0)
            {
                // 1단 점프 (스태미나 소모 X)
                _yVelocity = JumpPower;
                _jumpCount = 1;
            }
            else if (_jumpCount == 1 && _currentStamina >= DoubleJumpStaminaCost)
            {
                // 2단 점프 (스태미나 소모 O)
                _yVelocity = JumpPower;
                _currentStamina -= DoubleJumpStaminaCost;
                _jumpCount = 2;
            }
        }

        // 3. 달리기 입력 확인
        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift);
        bool isSprinting = wantsToSprint && isMoving && _currentStamina > 0;

        // 4. 스태미나 처리
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

        // 5. 이동 속도 결정
        float currentSpeed = isSprinting ? SprintSpeed : MoveSpeed;

        // 6. 입력에 따른 방향 구하기
        // 카메라의 전방/우측 벡터 get
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // 이동 방향 계산
        Vector3 moveDirection = (right * x + forward * y).normalized;

        // 7. 수평,수직 분리
        Vector3 movement = moveDirection * currentSpeed;
        movement.y = _yVelocity;
        
        _characterController.Move(movement * Time.deltaTime);
    }
}
