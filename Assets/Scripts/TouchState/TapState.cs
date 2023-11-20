using UnityEngine;

public class TapState : TouchState
{
    private float holdTime = 0.3f;

    private float TouchDistance
    {
        get
        {
            return Vector3.Distance(startPosition, endPosition);
        }
    }

    public override void Enter()
    {
        Debug.Log("Tap");
        startPosition = Input.GetTouch(0).position;
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer > holdTime)
        {
            PlayerController.Instance.SetState(PlayerController.States.Hold);
        }
    }

    public override void Exit()
    {
        if (timer > holdTime)
        {
            Debug.Log("Tap End");
        }
        else
        {
            Debug.Log("Tap Action");
        }
        endPosition = Input.GetTouch(0).position;
        if (SwipeDetected)
        {
            Debug.Log($"Swipe!!!! {startPosition - endPosition}");
        }

        startPosition = Vector2.zero;
        endPosition = Vector2.zero;
        timer = 0f;
    }
}
