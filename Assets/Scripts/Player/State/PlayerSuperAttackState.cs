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
        switch (controller.player.attackState)
        {
            case Player.AttackState.Before:
                break;
            case Player.AttackState.Attack:
                break;
            case Player.AttackState.AfterStart:

            case Player.AttackState.AfterEnd:
                break;
            case Player.AttackState.End:
                controller.SetState(PlayerController.State.Idle);
                break;
        }
    }

    public override void FixedUpdate()
    {

    }

    public override void Exit()
    {
        controller.player.Animator.ResetTrigger(triggerName);
    }
}
