using UnityEngine;

public class PlayerIdleState : PlayerStateBase
{
    public PlayerIdleState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        //controller.player.Animator.Play("Idle");
        if (controller.nextState != PlayerController.State.Idle)
        {
            controller.SetState(controller.nextState);
            controller.nextState = PlayerController.State.Idle;
        }
    }

    public override void Update()
    {
        
    }

    public override void FixedUpdate()
    {
    }

    public override void Exit()
    {
    }
}
