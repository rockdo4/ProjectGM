using UnityEngine;

public class PlayerSprintState : PlayerStateBase
{
    private Vector3 direction = Vector3.forward;
    private Vector3 startPosition;

    public PlayerSprintState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        controller.player.animator.SetTrigger("Sprint");

        startPosition = controller.player.rigid.position;
    }

    public override void Update()
    {

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

            if (controller.player.DistanceToEnemy < controller.player.attackRange)
            {
                controller.SetState(PlayerController.State.Attack);
            }
        }
        else
        {
            controller.SetState(PlayerController.State.Idle);
        }
    }

    public override void Exit()
    {
    }
}
