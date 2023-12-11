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
        controller.MoveWeaponPosition(PlayerController.WeaponPosition.Wing);

        controller.player.Animator.Play("Idle");
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
        controller.player.Animator.SetTrigger("Evade");

        startPosition = controller.player.Rigid.position;
    }

    public override void Update()
    {
        controller.player.evadeTimer += Time.deltaTime;

        if (controller.player.evadeTimer > controller.player.Stat.evadeTime)
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
            var moveSpeed = controller.player.stat.MoveSpeed;
            controller.player.Rigid.MovePosition(position + rotation * direction * moveSpeed * Time.fixedDeltaTime);
        }
    }

    public override void Exit()
    {
        controller.player.Animator.ResetTrigger("Evade");
    }
}
