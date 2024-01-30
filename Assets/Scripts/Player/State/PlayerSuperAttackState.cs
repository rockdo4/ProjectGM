using UnityEngine;

public class PlayerSuperAttackState : PlayerStateBase
{
    private Animator animator;
    private const string triggerName = "SuperAttack";
    private bool isPlay = false;

    public PlayerSuperAttackState(PlayerController controller) : base(controller)
    {
    }

    public override void Enter()
    {
        isPlay = false;

        animator ??= controller.player.Animator;
        animator.speed = controller.player.Stat.globalSpeed.attackSpeed * controller.player.Stat.attackSpeed;

        //controller.MoveWeaponPosition(PlayerController.WeaponPosition.Hand);
        controller.player.Animator.SetTrigger(triggerName);
    }

    public override void Update()
    {
        if (!isPlay && animator.GetCurrentAnimatorStateInfo(0).IsName(triggerName))
        {
            isPlay = true;
            controller.player.Enemy.GetComponent<EnemyAI>().grogyTimer = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        }
        if (!isPlay)
        {
            return;
        }

        switch (controller.player.attackState)
        {
            case Player.AttackState.Before:
                break;
            case Player.AttackState.Attack:
                break;
            case Player.AttackState.AfterStart:
                break;
            case Player.AttackState.AfterEnd:
                break;
            case Player.AttackState.End:
                controller.player.GroggyAttack = false;
                controller.SetState(PlayerController.State.Idle);
                break;
        }
    }

    public override void FixedUpdate()
    {

    }

    public override void Exit()
    {
        controller.player.attackState = Player.AttackState.None;
        controller.player.Animator.ResetTrigger(triggerName);
    }
}
