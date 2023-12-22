using UnityEngine;

public class PlayerSprintState : PlayerStateBase
{
    private Animator animator;
    private Vector3 direction = Vector3.forward;
    private AnimationClip animation;
    private const string triggerName = "Sprint";

    private const float castingTime = 0.05f;
    private float castingTimer = 0f;
    private bool isCast = false;

    public PlayerSprintState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        castingTimer = 0f;
        isCast = false;

        animator ??= controller.player.Animator;
        animator.speed = controller.player.Stat.globalSpeed.sprintSpeed;

        if (controller.player.CanAttack)
        {
            AttackCheck();
            return;
        }

        controller.MoveWeaponPosition(PlayerController.WeaponPosition.Wing);
    }

    public override void Update()
    {
        if (!isCast)
        {
            castingTimer += Time.deltaTime;
            if (castingTimer >= castingTime)
            {
                controller.player.Animator.SetTrigger(triggerName);
                controller.player.effects.PlayEffect(PlayerEffectType.Sprint);
                isCast = true;
            }
            return;
        }

        if (!controller.player.Animator.IsInTransition(0))
        {
            animation = controller.player.Animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        }
    }

    public override void FixedUpdate()
    {
        if (!isCast)
        {
            return;
        }
        if (animation == null)
        {
            return;
        }
        controller.player.Rigid.velocity = Vector3.zero;

        var rotation = Quaternion.Euler(0, controller.player.Rigid.rotation.eulerAngles.y, controller.player.Rigid.rotation.eulerAngles.z);
        float speed = controller.player.Stat.moveDistance * animator.speed / animation.length;
        var force = rotation * direction * speed;
        controller.player.Rigid.AddForce(force, ForceMode.VelocityChange);

        if (controller.player.CanAttack)
        {
            AttackCheck();
            return;
        }
    }

    public override void Exit()
    {
        controller.player.effects.StopEffect(PlayerEffectType.Sprint);
        controller.player.Animator.ResetTrigger("Sprint");
    }

    private void AttackCheck()
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
