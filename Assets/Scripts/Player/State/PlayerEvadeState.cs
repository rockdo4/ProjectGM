using UnityEngine;

public class PlayerEvadeState : PlayerStateBase
{
    private Vector3 direction;
    private Vector3 startPosition;

    public PlayerEvadeState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        controller.player.animator.Play("Idle");
        controller.player.evadeTimer = 0f;
        direction = TouchManager.Instance.swipeDirection switch
        {
            TouchManager.SwipeDirection.Up => Vector3.forward,
            TouchManager.SwipeDirection.Down => Vector3.back,
            TouchManager.SwipeDirection.Left => Vector3.left,
            TouchManager.SwipeDirection.Right => Vector3.right,
            _ => Vector3.zero
        };

        controller.player.animator.SetFloat("X", direction.x);
        controller.player.animator.SetFloat("Z", direction.z);
        controller.player.animator.SetTrigger("Evade");

        startPosition = controller.player.rigid.position;
    }

    public override void Update()
    {
        controller.player.evadeTimer += Time.deltaTime;

        if (controller.player.evadeTimer > controller.player.stat.evadeTime)
        {
            controller.SetState(PlayerController.State.Idle);
        }
    }

    public override void FixedUpdate()
    {
        var position = controller.player.rigid.position;
        if (Vector3.Distance(startPosition, position) < controller.player.MoveDistance)
        {
            var rotation = controller.player.rigid.rotation;
            rotation.x = 0f;
            var moveSpeed = controller.player.stat.MoveSpeed;
            controller.player.rigid.MovePosition(position + rotation * direction * moveSpeed * Time.deltaTime);
        }
    }

    public override void Exit()
    {
        controller.player.animator.ResetTrigger("Evade");
    }
}
