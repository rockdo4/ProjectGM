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
        controller.MoveWeapon(PlayerController.WeaponPosition.Wing);

        controller.player.Animator.SetTrigger("Sprint");

        startPosition = controller.player.Rigid.position;
    }

    public override void Update()
    {

    }

    public override void FixedUpdate()
    {
        var position = controller.player.Rigid.position;
        if (Vector3.Distance(startPosition, position) < controller.player.MoveDistance)
        {
            var rotation = controller.player.Rigid.rotation;
            rotation.x = 0f;

            var moveSpeed = controller.player.stat.MoveSpeed;
            controller.player.Rigid.MovePosition(position + rotation * direction * moveSpeed * Time.deltaTime);

            if (controller.player.DistanceToEnemy < controller.player.CurrentWeapon.attackRange)
            {
                if (controller.player.Enemy.IsGroggy)
                {
                    controller.SetState(PlayerController.State.SuperAttack);
                }
                else
                {
                    controller.SetState(PlayerController.State.Attack);
                }
                //controller.SetState((controller.player.Enemy.IsGroggy) ? PlayerController.State.SuperAttack : PlayerController.State.Attack);
            }
        }
        else
        {
            controller.SetState(PlayerController.State.Idle);
        }
    }

    public override void Exit()
    {
        controller.player.Animator.ResetTrigger("Sprint");
    }
}
