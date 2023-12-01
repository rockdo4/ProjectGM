using Unity.VisualScripting;
using UnityEngine;

public class PlayerHitState : PlayerStateBase
{
    private const string triggerName = "Hit";

    public PlayerHitState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        controller.player.Animator.SetTrigger(triggerName);
        
    }

    public override void Update()
    {
        var animation = controller.player.Animator.GetCurrentAnimatorStateInfo(0);
        if (animation.normalizedTime >= 1f)
        {
            controller.SetState(PlayerController.State.Idle);
        }
    }

    public override void FixedUpdate()
    {
    }

    public override void Exit()
    {
        controller.player.IsGroggy = false;
        controller.player.Animator.ResetTrigger(triggerName);
    }
}
