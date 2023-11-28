using UnityEngine;

public class PlayerAttackState : PlayerStateBase
{
    private bool isSuperAttack = false;
    private bool isFirst = false;

    public PlayerAttackState(PlayerController controller) : base(controller)
    {
    }

    public override void Enter()
    {
        if (controller.player.Enemy.GetComponent<TempEnemy>().isGroggy)
        {
            controller.player.isAttack = true;
            controller.player.canCombo = true;
            controller.player.Animator.SetTrigger("SuperAttack");
            isSuperAttack = true;
            return;
        }

        isFirst = true;
    }

    public override void Update()
    {
        if (!TouchManager.Instance.Holded)
        {
            controller.SetState(PlayerController.State.Idle);
        }

        if (isFirst || (controller.player.canCombo && !controller.player.isAttack))
        {
            if (isFirst)
            {
                isFirst = false;
            }
            controller.player.Animator.SetTrigger("Attack");
        }
    }

    public override void FixedUpdate()
    {

    }

    public override void Exit()
    {
        if (isSuperAttack)
        {
            controller.player.Enemy.GetComponent<TempEnemy>().isGroggy = false;
            isSuperAttack = false;
        }

        controller.player.Animator.ResetTrigger("Attack");
    }
}
