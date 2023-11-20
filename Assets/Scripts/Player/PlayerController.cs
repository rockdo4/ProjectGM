using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    private StateManager stateManager = new StateManager();
    public enum States
    {
        Idle,
        Tap,
        Hold,
        UpSwipe,
        DownSwipe,
        LeftSwipe,
        RightSwipe,
    }
    private List<StateBase> states = new List<StateBase>();
    public int ComboCount { get; set; }

    private void Start()
    {
        states.Add(new IdleState());
        states.Add(new TapState());
        states.Add(new HoldState());
        states.Add(new SwipeUpState());
        states.Add(new SwipeDownState());
        states.Add(new SwipeLeftState());
        states.Add(new SwipeRightState());

        SetState(States.Idle);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            SetState(States.Idle);
        }

        stateManager.Update();
    }

    public void SetState(States newState)
    {
        stateManager.ChangeState(states[(int)newState]);
    }

    //Animation Event
    public void ResetCombo()
    {
        ComboCount = 0;
    }
}
