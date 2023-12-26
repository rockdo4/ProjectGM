using System.Net.Http.Headers;
using UnityEngine;

public class PlayerEvadeState : PlayerStateBase
{
    private Animator animator;
    private Vector3 direction;
    private const string triggerName = "Evade";
    private AnimationClip animation;
    private float distance;

    public PlayerEvadeState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        animator ??= controller.player.Animator;
        animator.speed = controller.player.Stat.globalSpeed.evadeSpeed;

        controller.MoveWeaponPosition(PlayerController.WeaponPosition.Wing);
        controller.player.evadeTimer = 0f;
        direction = TouchManager.Instance.swipeDirection switch
        {
            TouchManager.SwipeDirection.Up => Vector3.forward,
            TouchManager.SwipeDirection.Down => Vector3.back,
            TouchManager.SwipeDirection.Left => Vector3.left,
            TouchManager.SwipeDirection.Right => Vector3.right,
            _ => Vector3.zero
        };

        distance = TouchManager.Instance.swipeDirection switch
        {
            TouchManager.SwipeDirection.Up => controller.player.Stat.frontBackEvadeDistance,
            TouchManager.SwipeDirection.Down => controller.player.Stat.frontBackEvadeDistance,
            TouchManager.SwipeDirection.Left => controller.player.Stat.leftRightEvadeDistance,
            TouchManager.SwipeDirection.Right => controller.player.Stat.leftRightEvadeDistance,
            _ => 0f
        };

        controller.player.Animator.SetFloat("X", direction.x);
        controller.player.Animator.SetFloat("Z", direction.z);
        controller.player.Animator.SetTrigger(triggerName);

        controller.player.Effects.PlayEffect(PlayerEffectType.Evade, direction);
    }

    public override void Update()
    {
        controller.player.evadeTimer += Time.deltaTime;

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
        float speed = distance * animator.speed / animation.length;
        var force = rotation * direction * speed;
        controller.player.Rigid.AddForce(force, ForceMode.VelocityChange);
    }

    public override void Exit()
    {

    }
}
