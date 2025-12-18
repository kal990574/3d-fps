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
    private bool _isJumping;

    // 달리기 관련
    [Header("Sprint")]
    public float StaminaDrainRate = 20f;

    // 컴포넌트 참조
    private PlayerStats _playerStats;
    private PlayerAnimationController _animController;
    private Camera _mainCamera;

    // 활성화 상태
    private bool _isActive = true;

    public bool IsActive => _isActive;

    // 애니메이션용 프로퍼티
    public bool IsGrounded => _characterController != null && _characterController.isGrounded;
    public float CurrentSpeed { get; private set; }
    public bool IsSprinting { get; private set; }

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _playerStats = GetComponent<PlayerStats>();
        _animController = GetComponent<PlayerAnimationController>();
        _mainCamera = Camera.main;

        if (_playerStats == null)
        {
            Debug.LogError("PlayerStats component not found!");
        }

        if (_mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
        }

        if (_animController != null)
        {
            _animController.OnJumpExecute += ExecuteJump;
        }
    }

    private void OnDestroy()
    {
        if (_animController != null)
        {
            _animController.OnJumpExecute -= ExecuteJump;
        }
    }

    private void ExecuteJump()
    {
        _yVelocity = JumpPower;
    }

    public void Enable()
    {
        _isActive = true;

        if (_characterController != null)
        {
            _characterController.enabled = true;
        }
    }

    public void Disable()
    {
        _isActive = false;
    }

    private void Update()
    {
        if (!_isActive)
        {
            return;
        }

        // 땅에 닿아있는지 체크
        bool isGrounded = _characterController.isGrounded;

        if (isGrounded)
        {
            if (!_isJumping)
            {
                _yVelocity = 0;
                _jumpCount = 0;
            }
        }
        else
        {
            _isJumping = false;
            _yVelocity -= Gravity * Time.deltaTime;
        }

        // 1. 키보드 입력 받기
        float x = InputManager.Instance.Horizontal;
        float y = InputManager.Instance.Vertical;
        bool isMoving = x != 0 || y != 0;

        // 2. 점프 처리
        if (InputManager.Instance.JumpPressed)
        {
            if (_jumpCount == 0)
            {
                // 1단 점프
                _jumpCount = 1;
                _isJumping = true;
                _animController?.TriggerJump();
            }
            else if (_jumpCount == 1 && !isGrounded && _playerStats.HasStamina(DoubleJumpStaminaCost))
            {
                // 2단 점프
                if (_playerStats.TryUseStamina(DoubleJumpStaminaCost))
                {
                    _jumpCount = 2;
                    _isJumping = true;
                    _animController?.TriggerJump();
                }
            }
        }

        // 3. 달리기 입력 확인
        bool wantsToSprint = InputManager.Instance.SprintHeld;
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

        // 애니메이션용 프로퍼티 갱신
        CurrentSpeed = isMoving ? currentSpeed : 0f;
        IsSprinting = isSprinting;
    }
}
