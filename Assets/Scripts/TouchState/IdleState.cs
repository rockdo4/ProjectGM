using UnityEngine;

public class IdleState : TouchState
{
    public override void Enter()
    {
        Debug.Log("Idle");
    }

    public override void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlayerController.Instance.SetState(PlayerController.States.Tap);
        }
    }

    public override void Exit()
    {
    }
}
