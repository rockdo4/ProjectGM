using UnityEngine;

public class PlayerAttackState : PlayerStateBase
{


    public PlayerAttackState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        Player2.Instance.anim.SetTrigger("Attack");
    }

    public override void Update()
    {

    }

    public override void FixedUpdate()
    {

    }

    public override void Exit()
    {

    }
}
