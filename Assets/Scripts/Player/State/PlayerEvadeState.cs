using UnityEngine;

public class PlayerEvadeState : PlayerStateBase
{
    private Vector3 direction;
    private Vector3 startPosition;
    private const string triggerName = "Evade";


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
        controller.player.Animator.Play("Idle");
        controller.player.Animator.SetFloat("X", direction.x);
        controller.player.Animator.SetFloat("Z", direction.z);
        controller.player.Animator.SetTrigger(triggerName);

        startPosition = controller.player.Rigid.position;

        controller.player.effects.PlayEffect(EffectType.Evade, direction);
    }

    public override void Update()
    {
        controller.player.evadeTimer += Time.deltaTime;

        var animationStateInfo = controller.player.Animator.GetCurrentAnimatorStateInfo(0);
        if (animationStateInfo.IsName(triggerName) && animationStateInfo.normalizedTime >= 1f)
        {
            controller.SetState(PlayerController.State.Idle);
        }

        //var position = controller.player.transform.position;
        //if (Vector3.Distance(startPosition, position) < controller.player.MoveDistance)
        //{
        //    var rotation = controller.player.transform.rotation;
        //    rotation.x = 0f;
        //    var moveSpeed = controller.player.stat.MoveSpeed;
        //    controller.player.transform.position = (position + rotation * direction * moveSpeed * Time.deltaTime);
        //}
    }

    public override void FixedUpdate()
    {
        var position = controller.player.Rigid.position;
        if (Vector3.Distance(startPosition, position) < controller.player.MoveDistance)
        {
            var rotation = controller.player.Rigid.rotation;
            rotation.x = 0f;
            var moveSpeed = controller.player.Stat.MoveSpeed;
            var force = rotation * direction * moveSpeed;
            controller.player.Rigid.AddForce(force, ForceMode.VelocityChange);
        }
    }
    //controller.player.Rigid.MovePosition(position + rotation * direction * moveSpeed * Time.fixedDeltaTime);

    public override void Exit()
    {
    }
}
