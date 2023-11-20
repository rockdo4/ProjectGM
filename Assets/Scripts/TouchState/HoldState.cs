using UnityEngine;

public class HoldState : TouchState
{
    public override void Enter()
    {
        Debug.Log("Hold");
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
        Debug.Log("Hold End");
    }
}