using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CameraModeSwitch _cameraModeSwitch;

    private PlayerMove _wasdMovement;
    private PlayerClickToMove _clickToMove;
    private bool _wasQuarterView;

    private void Start()
    {
        _wasdMovement = GetComponent<PlayerMove>();
        _clickToMove = GetComponent<PlayerClickToMove>();

        if (_wasdMovement == null)
        {
            Debug.LogError("PlayerMove component not found!");
        }

        if (_clickToMove == null)
        {
            Debug.LogError("PlayerClickToMove component not found!");
        }

        if (_cameraModeSwitch == null)
        {
            Debug.LogError("CameraModeSwitch reference not set!");
        }

        // 초기 상태: WASD 모드 활성화
        if (_clickToMove != null)
        {
            _clickToMove.Disable();
        }
    }

    private void Update()
    {
        if (_cameraModeSwitch == null)
        {
            return;
        }

        bool isQuarterView = _cameraModeSwitch.IsQuarterView;

        // 모드 전환 감지
        if (isQuarterView != _wasQuarterView)
        {
            SwitchMovementMode(isQuarterView);
            _wasQuarterView = isQuarterView;
        }

        // 쿼터뷰에서 우클릭 시 이동
        if (isQuarterView && InputManager.Instance.RightClickPressed)
        {
            _clickToMove.TryMoveToMousePosition();
        }
    }

    private void SwitchMovementMode(bool useClickToMove)
    {
        if (useClickToMove)
        {
            if (_wasdMovement != null)
            {
                _wasdMovement.Disable();
            }

            if (_clickToMove != null)
            {
                _clickToMove.Enable();
            }
        }
        else
        {
            if (_clickToMove != null)
            {
                _clickToMove.Disable();
            }

            if (_wasdMovement != null)
            {
                _wasdMovement.Enable();
            }
        }
    }
}