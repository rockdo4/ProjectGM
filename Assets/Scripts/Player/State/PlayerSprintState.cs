using UnityEngine;
using UnityEngine.UIElements;

public class PlayerSprintState : PlayerStateBase
{
    private Vector3 direction = Vector3.forward;
    private AnimationClip animation;

    public PlayerSprintState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        controller.MoveWeaponPosition(PlayerController.WeaponPosition.Wing);
        controller.player.Animator.SetTrigger("Sprint");
    }

    public override void Update()
    {
        if (!controller.player.Animator.IsInTransition(0))
        {
            animation = controller.player.Animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        }
    }

    public override void FixedUpdate()
    {
        if (animation == null)
        {
            return;
        }
        if (controller.player.CanAttack)
        {
            if (controller.player.Enemy.IsGroggy)
            {
                controller.SetState(PlayerController.State.SuperAttack);
            }
            else
            {
                controller.SetState(PlayerController.State.Attack);
            }
        }

        var rotation = Quaternion.Euler(0, controller.player.Rigid.rotation.eulerAngles.y, controller.player.Rigid.rotation.eulerAngles.z);
        float speed = controller.player.MoveDistance / animation.length;
        var force = rotation * direction * speed;
        controller.player.Rigid.AddForce(force, ForceMode.VelocityChange);
    }

    public override void Exit()
    {
        controller.player.Animator.ResetTrigger("Sprint");
    }
}
