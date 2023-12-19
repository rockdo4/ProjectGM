using UnityEngine;

public class PlayerSuperAttackState : PlayerStateBase
{
    private Animator animator;
    private const string triggerName = "SuperAttack";

    public PlayerSuperAttackState(PlayerController controller) : base(controller)
    {
    }

    public override void Enter()
    {
        animator ??= controller.player.Animator;
        animator.speed = controller.player.Stat.globalSpeed.attackSpeed * controller.player.Stat.attackSpeed;

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
