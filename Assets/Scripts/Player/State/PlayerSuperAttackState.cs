public class PlayerSuperAttackState : PlayerStateBase
{
    private const string triggerName = "SuperAttack";
    
    public PlayerSuperAttackState(PlayerController controller) : base(controller)
    {
    }

    public override void Enter()
    {
        controller.MoveWeaponPosition(PlayerController.WeaponPosition.Hand);
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
        controller.player.Animator.ResetTrigger(triggerName);
    }
}
