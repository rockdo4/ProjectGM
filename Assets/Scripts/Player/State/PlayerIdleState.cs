using UnityEngine;

public class PlayerIdleState : PlayerStateBase
{
    public PlayerIdleState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
    }

    public override void Update()
    {
        if (TouchManager.Instance.Swiped)
        {
            //Player2.Instance.SetState(Player2.States.Evade);
        }
    }

    public override void FixedUpdate()
    {
    }

    public override void Exit()
    {
    }
}
