using UnityEngine;

public abstract class StateBase
{
    abstract public void Enter();
    abstract public void Update();
    abstract public void Exit();
}

public abstract class TouchState : StateBase
{
    protected float timer = 0f;

    protected Vector2 startPosition;
    protected Vector2 endPosition;
    protected float swipeDistance = 0.2f;

    protected bool SwipeDetected
    {
        get
        {
            return Vector2.Distance(startPosition.normalized, endPosition.normalized) >= swipeDistance;
        }
    }
}