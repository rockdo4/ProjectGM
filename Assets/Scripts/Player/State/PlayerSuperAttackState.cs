public class PlayerSuperAttackState : PlayerStateBase
{
    private const string triggerName = "SuperAttack";
    
    public PlayerSuperAttackState(PlayerController controller) : base(controller)
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
        controller.player.isAttack = false;
        controller.player.Animator.ResetTrigger(triggerName);
    }
}
