using UnityEngine;

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
    public float DoubleJumpStaminaCost = 25f;
    private int _jumpCount = 0;

    // 달리기 관련
    [Header("Sprint")]
    public float StaminaDrainRate = 20f;

    // 컴포넌트 참조
    private PlayerStats _playerStats;
    private Camera _mainCamera;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _playerStats = GetComponent<PlayerStats>();
        _mainCamera = Camera.main;

        if (_playerStats == null)
        {
            Debug.LogError("PlayerStats component not found!");
        }

        if (_mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
        }
    }

    private void Update()
    {
        // 땅에 닿아있는지 체크
        bool isGrounded = _characterController.isGrounded;

        if (isGrounded)
        {
            _yVelocity = 0;
            _jumpCount = 0;
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
            else if (_jumpCount == 1 && _playerStats.HasStamina(DoubleJumpStaminaCost))
            {
                // 2단 점프 (스태미나 소모 O)
                if (_playerStats.TryUseStamina(DoubleJumpStaminaCost))
                {
                    _yVelocity = JumpPower;
                    _jumpCount = 2;
                }
            }
        }

        // 3. 달리기 입력 확인
        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift);
        bool isSprinting = wantsToSprint && isMoving && _playerStats.HasStamina(0.1f);

        // 4. 스태미나 처리
        if (isSprinting)
        {
            // 달리는 중 - 스태미나 소모
            _playerStats.UseStamina(StaminaDrainRate, Time.deltaTime);
        }
        // 달리지 않을 때는 PlayerStats에서 자동 회복

        // 5. 이동 속도 결정
        float currentSpeed = isSprinting ? SprintSpeed : MoveSpeed;

        // 6. 입력에 따른 방향 구하기
        Vector3 forward = _mainCamera.transform.forward;
        Vector3 right = _mainCamera.transform.right;
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
