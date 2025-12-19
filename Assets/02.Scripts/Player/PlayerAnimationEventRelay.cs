using UnityEngine;

public class PlayerAnimationEventRelay : MonoBehaviour
{
    private PlayerAnimationController _controller;

    private void Awake()
    {
        _controller = GetComponentInParent<PlayerAnimationController>();

        if (_controller == null)
        {
            Debug.LogError("PlayerAnimationController not found in parent!");
        }
    }

    public void AnimEvent_Jump()
    {
        _controller?.AnimEvent_Jump();
    }

    public void AnimEvent_Throw()
    {
        Debug.Log("AnimEvent_Throw called");
        _controller?.AnimEvent_Throw();
    }
}