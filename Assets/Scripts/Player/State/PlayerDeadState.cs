using UnityEngine;

public class PlayerDeadState : PlayerStateBase
{
    private const string triggerName = "Death";
    public PlayerDeadState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        controller.player.Animator.SetTrigger(triggerName);
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
