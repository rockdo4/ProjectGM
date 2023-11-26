using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerStateBase
{
    private enum ComboAnimation
    {
        None,
        Combo_04_1,
        Combo_04_2,
        Combo_04_3,
        Combo_04_4,
        Count,
    }
    private ComboAnimation currentCombo = ComboAnimation.None;
    private bool isAnimationPlaying = false;

    public PlayerAttackState(PlayerController controller) : base(controller)
    {
    }

    public override void Enter()
    {
        currentCombo = ComboAnimation.None;
        controller.player.canCombo = true;
        isAnimationPlaying = false;

        SetCombo(ComboAnimation.Combo_04_1);
    }

    public override void Update()
    {
        if (isAnimationPlaying)
        {
            var animatorStateInfo = controller.player.anim.GetCurrentAnimatorClipInfo(0);
            var clip = animatorStateInfo[0].clip;
            if (controller.player.anim.GetCurrentAnimatorStateInfo(0).IsName(currentCombo.ToString()))
            {
                if (controller.player.canCombo && controller.player.isAttack)
                {
                    if (currentCombo == ComboAnimation.Count - 1)
                    {
                        currentCombo = ComboAnimation.None;
                    }
                    SetCombo(currentCombo + 1);
                }
            }
        }

        if (!controller.player.canCombo)
        {
            controller.SetState(PlayerController.State.Idle);
            return;
        }
    }

    public override void FixedUpdate()
    {

    }

    public override void Exit()
    {

    }
    
    private void SetCombo(ComboAnimation newCombo)
    {
        if (currentCombo == newCombo)
        {
            return;
        }
        controller.player.anim.SetTrigger("Attack");
        currentCombo = newCombo;
        isAnimationPlaying = true;
    }
}
