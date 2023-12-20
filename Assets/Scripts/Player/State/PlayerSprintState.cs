using UnityEngine;

public class PlayerSprintState : PlayerStateBase
{
    private Animator animator;
    private Vector3 direction = Vector3.forward;
    private AnimationClip animation;

    public PlayerSprintState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        animator ??= controller.player.Animator;
        animator.speed = controller.player.Stat.globalSpeed.sprintSpeed;

        if (controller.player.CanAttack)
        {
            AttackCheck();
            return;
        }

        controller.MoveWeaponPosition(PlayerController.WeaponPosition.Wing);
        controller.player.Animator.SetTrigger("Sprint");

        controller.player.effects.PlayEffect(PlayerEffectType.Sprint);
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
        controller.player.Rigid.velocity = Vector3.zero;

        var rotation = Quaternion.Euler(0, controller.player.Rigid.rotation.eulerAngles.y, controller.player.Rigid.rotation.eulerAngles.z);
        float speed = controller.player.MoveDistance * animator.speed / animation.length;
        var force = rotation * direction * speed;
        controller.player.Rigid.AddForce(force, ForceMode.VelocityChange);

        AttackCheck();
    }

    public override void Exit()
    {
        controller.player.effects.StopEffect(PlayerEffectType.Sprint);
        controller.player.Animator.ResetTrigger("Sprint");
    }

    private void AttackCheck()
    {
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
    }
}
