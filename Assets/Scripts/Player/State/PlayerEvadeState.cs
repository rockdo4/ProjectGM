using UnityEngine;

public class PlayerEvadeState : PlayerStateBase
{
    private Vector3 direction;
    private Vector3 startPosition;
    private const string triggerName = "Evade";

    private float animationDuration;
    public PlayerEvadeState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
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
        controller.player.Animator.SetFloat("X", direction.x);
        controller.player.Animator.SetFloat("Z", direction.z);
        controller.player.Animator.SetTrigger(triggerName);

        startPosition = controller.player.Rigid.position;

        animationDuration = controller.player.Animator.GetCurrentAnimatorStateInfo(0).length;
        Debug.Log(animationDuration);
        controller.player.effects.PlayEffect(EffectType.Evade, direction);
    }

    public override void Update()
    {
        controller.player.evadeTimer += Time.deltaTime;
    }

    public override void FixedUpdate()
    {
        var position = controller.player.Rigid.position;
        var rotation = controller.player.Rigid.rotation;
        rotation.x = 0f;
        float distanceToMovePerFrame = controller.player.MoveDistance / (19f / 30f);
        var force = rotation * direction;
        controller.player.Rigid.AddForce(force * distanceToMovePerFrame, ForceMode.VelocityChange);

        if (Vector3.Distance(startPosition, position) >= controller.player.MoveDistance)
        {
            controller.SetState(PlayerController.State.Idle);
        }
    }
    public override void Exit()
    {
    }
}
