using UnityEngine;

public class MonsterAnimationEventRelay : MonoBehaviour
{
    private MonsterAnimationController _controller;

    private void Awake()
    {
        _controller = GetComponentInParent<MonsterAnimationController>();

        if (_controller == null)
        {
            Debug.LogError("MonsterAnimationController not found in parent!");
        }
    }

    // Animation Event 콜백
    public void AnimEvent_Attack()
    {
        Debug.Log("MonsterAnimationEventRelay.AnimEvent_Attack called");
        _controller?.AnimEvent_Attack();
    }
}