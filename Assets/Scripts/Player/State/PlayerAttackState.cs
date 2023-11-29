using UnityEngine;

public class PlayerAttackState : PlayerStateBase
{
    private const string triggerName = "Attack";
    private bool isFirst = false;

    public PlayerAttackState(PlayerController controller) : base(controller)
    {
    }

    public override void Enter()
    {
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
            controller.player.Animator.SetTrigger(triggerName);
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
