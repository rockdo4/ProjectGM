using UnityEngine;

public class PlayerIdleState : PlayerStateBase
{
    public PlayerIdleState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        //controller.player.Animator.Play("Idle");
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
